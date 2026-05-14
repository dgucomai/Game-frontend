using System;
using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 아코 키우기 게임 로직의 핵심.
    /// 
    /// 강화 흐름:
    ///   1. UI가 TryEnhance() 호출
    ///   2. 코인 1개 차감 (0단계에서 시도 시에만)
    ///   3. 확률로 성공/실패 결정
    ///      - 성공 + 다음이 마지막 단계가 아님: EnhanceResult.Success
    ///      - 성공 + 다음이 마지막 단계(isGrowthCompletion): EnhanceResult.GrowthCompleted
    ///      - 실패 + 현 단계의 protectionCost > 0 + 보유 방지권 >= protectionCost:
    ///        EnhanceResult.FailedCanUseProtection → UI가 Yes/No 묻고 ConfirmUseProtection 호출
    ///      - 그 외 실패: 즉시 0단계 리셋, EnhanceResult.FailedReset
    /// </summary>
    public class AkoGameManager : MonoBehaviour
    {
        public static AkoGameManager Instance { get; private set; }

        [Header("데이터")]
        [SerializeField] private AkoDatabase database;

        [Header("게임 설정")]
        [Tooltip("0단계에서 강화 시도 시 차감되는 코인")]
        [SerializeField] private int gameStartCost = 1;

        public AkoSaveData Save { get; private set; }
        public ICoinService Coins => AkoCoinHolder.Service;

        // 방지권 사용 결정 대기 중인지 (FailedCanUseProtection 상태)
        private int pendingFailedLevel = -1;

        private IAkoLeaderboardService leaderboard;
        public int GlobalBestLevel { get; private set; } = 0;

        // ===== 이벤트 =====
        public event Action<int> OnLevelChanged;
        public event Action<int> OnProtectionChanged;
        public event Action<EnhanceResult> OnEnhanceResult;
        public event Action<int> OnGlobalBestChanged;
        public event Action<AkoStageData> OnSold; // 판매 단계 정보 전달

        // ============================================================
        //  라이프사이클
        // ============================================================
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Save = AkoSaveManager.Load();
            leaderboard = new MockLeaderboardService();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            OnLevelChanged?.Invoke(Save.currentLevel);
            OnProtectionChanged?.Invoke(Save.protectionTickets);
            leaderboard.GetGlobalBest(best =>
            {
                GlobalBestLevel = best;
                OnGlobalBestChanged?.Invoke(best);
            });
        }

        // ============================================================
        //  강화
        // ============================================================
        public void TryEnhance()
        {
            if (database == null)
            {
                Debug.LogError("[AkoGameManager] database가 비어있음!");
                return;
            }

            if (pendingFailedLevel != -1) return; // 방지권 응답 대기 중

            bool isNewGame = (Save.currentLevel == 0);
            if (isNewGame && Coins.GetCoins() < gameStartCost)
            {
                Debug.Log("[Ako] 코인 부족!");
                return;
            }

            if (Save.currentLevel >= database.MaxLevel)
            {
                Debug.Log("[Ako] 이미 최대 단계.");
                return;
            }

            if (isNewGame && !Coins.SpendCoins(gameStartCost))
            {
                Debug.LogWarning("[Ako] 코인 차감 실패");
                return;
            }

            var currentStage = database.GetStage(Save.currentLevel);
            float roll = UnityEngine.Random.Range(0f, 100f);
            bool success = roll < currentStage.successRate;

            if (success)
            {
                Save.currentLevel++;

                // 개인 최고
                if (Save.currentLevel > Save.personalBestLevel)
                {
                    Save.personalBestLevel = Save.currentLevel;
                    leaderboard.SubmitMyBest(Save.currentLevel);
                }

                OnLevelChanged?.Invoke(Save.currentLevel);

                // 도달한 단계가 성장 완료 단계인지 확인 (+25 늙코)
                var reachedStage = database.GetStage(Save.currentLevel);
                if (reachedStage != null && reachedStage.isGrowthCompletion)
                {
                    OnEnhanceResult?.Invoke(EnhanceResult.GrowthCompleted);
                    // 리셋은 UI에서 모달 닫을 때 ResetToZero() 호출하여 처리
                }
                else
                {
                    OnEnhanceResult?.Invoke(EnhanceResult.Success);
                }

                AkoSaveManager.Save(Save);
            }
            else
            {
                // 실패: 방지권 사용 가능한지 확인
                bool canUseProtection = currentStage.protectionCost > 0
                                         && Save.protectionTickets >= currentStage.protectionCost;

                if (canUseProtection)
                {
                    pendingFailedLevel = Save.currentLevel;
                    OnEnhanceResult?.Invoke(EnhanceResult.FailedCanUseProtection);
                }
                else
                {
                    Save.currentLevel = 0;
                    OnLevelChanged?.Invoke(Save.currentLevel);
                    OnEnhanceResult?.Invoke(EnhanceResult.FailedReset);
                    AkoSaveManager.Save(Save);
                }
            }
        }

        /// <summary>
        /// 강화 실패 시 방지권 사용 여부 응답.
        /// </summary>
        public void ConfirmUseProtection(bool useIt)
        {
            if (pendingFailedLevel == -1)
            {
                Debug.LogWarning("[Ako] 방지권 사용 대기 상태가 아닙니다.");
                return;
            }

            var stage = database.GetStage(pendingFailedLevel);
            int cost = stage != null ? stage.protectionCost : 1;

            if (useIt && Save.protectionTickets >= cost)
            {
                Save.protectionTickets -= cost;
                OnProtectionChanged?.Invoke(Save.protectionTickets);
                Debug.Log($"[Ako] 방지권 {cost}개 사용! 단계 유지");
                // 단계 변동 없음
            }
            else
            {
                Save.currentLevel = 0;
                OnLevelChanged?.Invoke(Save.currentLevel);
                OnEnhanceResult?.Invoke(EnhanceResult.FailedReset);
            }

            pendingFailedLevel = -1;
            AkoSaveManager.Save(Save);
        }

        /// <summary>
        /// 성장 완료 모달 닫을 때 호출. 0단계로 리셋.
        /// </summary>
        public void ResetToZero()
        {
            Save.currentLevel = 0;
            OnLevelChanged?.Invoke(Save.currentLevel);
            AkoSaveManager.Save(Save);
        }

        // ============================================================
        //  판매
        // ============================================================
        public bool SellCurrentAko()
        {
            var stage = database.GetStage(Save.currentLevel);
            if (stage == null || !stage.isSellable) return false;

            // 코인 보상
            if (stage.sellRewardCoins > 0)
                Coins.AddCoins(stage.sellRewardCoins);

            // 아이템 보상
            if (!string.IsNullOrEmpty(stage.sellRewardItem))
                Save.ownedItems.Add(stage.sellRewardItem);

            // 단계 리셋
            int soldLevel = Save.currentLevel;
            Save.currentLevel = 0;
            OnLevelChanged?.Invoke(Save.currentLevel);
            OnSold?.Invoke(stage); // 단계 정보 그대로 전달
            AkoSaveManager.Save(Save);
            return true;
        }

        // ============================================================
        //  상점
        // ============================================================
        public bool BuyProtectionTickets(int amount, int totalCost)
        {
            if (!Coins.SpendCoins(totalCost)) return false;
            Save.protectionTickets += amount;
            OnProtectionChanged?.Invoke(Save.protectionTickets);
            AkoSaveManager.Save(Save);
            return true;
        }

        public bool UseWarpTicket(int targetLevel, int cost)
        {
            if (targetLevel < 0 || targetLevel > database.MaxLevel) return false;
            if (!Coins.SpendCoins(cost)) return false;

            Save.currentLevel = targetLevel;
            if (Save.currentLevel > Save.personalBestLevel)
            {
                Save.personalBestLevel = Save.currentLevel;
                leaderboard.SubmitMyBest(Save.currentLevel);
            }
            OnLevelChanged?.Invoke(Save.currentLevel);
            AkoSaveManager.Save(Save);
            return true;
        }

        // ============================================================
        //  헬퍼
        // ============================================================
        public AkoStageData GetCurrentStage() => database.GetStage(Save.currentLevel);
        public float GetCurrentSuccessRate() => GetCurrentStage()?.successRate ?? 0f;
        public int GetMaxLevel() => database != null ? database.MaxLevel : 0;
        public bool CanSellNow() => GetCurrentStage()?.isSellable ?? false;
        public bool IsWaitingForProtectionChoice() => pendingFailedLevel != -1;
        public int GetPendingProtectionCost()
        {
            if (pendingFailedLevel == -1) return 0;
            var stage = database.GetStage(pendingFailedLevel);
            return stage != null ? stage.protectionCost : 1;
        }

        // ============================================================
        //  디버그
        // ============================================================
        [ContextMenu("Debug/Reset Save")]
        public void DebugResetSave()
        {
            AkoSaveManager.Clear();
            Save = new AkoSaveData();
            (AkoCoinHolder.Service as MockCoinService)?.Reset();
            OnLevelChanged?.Invoke(Save.currentLevel);
            OnProtectionChanged?.Invoke(Save.protectionTickets);
        }

        [ContextMenu("Debug/Add 100 Coins")]
        public void DebugAddCoins()
        {
            AkoCoinHolder.Service.AddCoins(100);
        }
    }
}
