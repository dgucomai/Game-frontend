using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ako
{
    /// <summary>
    /// 메인 게임 화면 UI 컨트롤러.
    /// </summary>
    public class AkoMainUI : MonoBehaviour
    {
        [Header("캐릭터 표시")]
        public Image characterImage;
        public Text levelNameText;
        public Text successRateText;

        [Header("기록 (선택)")]
        public Text personalBestText;
        public Text globalBestText;

        [Header("하단 정보")]
        public Text protectionText;
        public Text coinText;

        [Header("버튼")]
        public Button upButton;
        public Button sellButton;
        public Button shopButton;

        [Header("강화 실패 모달")]
        public GameObject failModal;
        public Button failModalCloseButton;

        [Header("방지권 사용 여부 모달")]
        public GameObject protectionModal;
        public Button protectionYesButton;
        public Button protectionNoButton;
        public Text protectionModalText;

        [Header("판매 결과 모달")]
        public GameObject soldModal;
        public Text soldMessageText;
        public Button soldModalCloseButton;

        [Header("성장 완료 모달 (+25 늙코)")]
        public GameObject growthModal;
        public Text growthMessageText;
        public Button growthCloseButton;

        [Header("씬 이름")]
        public string shopSceneName = "3.AkoShop";

        private AkoGameManager gm;

        private void Start()
        {
            gm = AkoGameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("[AkoMainUI] AkoGameManager가 씬에 없습니다!");
                return;
            }

            gm.OnLevelChanged += OnLevelChanged;
            gm.Coins.OnCoinsChanged += OnCoinsChanged;
            gm.OnProtectionChanged += OnProtectionChanged;
            gm.OnEnhanceResult += OnEnhanceResult;
            gm.OnGlobalBestChanged += OnGlobalBestChanged;
            gm.OnSold += OnSold;

            if (upButton != null) upButton.onClick.AddListener(OnClickUp);
            if (sellButton != null) sellButton.onClick.AddListener(OnClickSell);
            if (shopButton != null) shopButton.onClick.AddListener(OnClickShop);
            if (failModalCloseButton != null)
                failModalCloseButton.onClick.AddListener(() => CloseModal(failModal));
            if (soldModalCloseButton != null)
                soldModalCloseButton.onClick.AddListener(() => CloseModal(soldModal));
            if (protectionYesButton != null)
                protectionYesButton.onClick.AddListener(OnClickProtectionYes);
            if (protectionNoButton != null)
                protectionNoButton.onClick.AddListener(OnClickProtectionNo);
            if (growthCloseButton != null)
                growthCloseButton.onClick.AddListener(OnClickGrowthClose);

            if (failModal != null) failModal.SetActive(false);
            if (protectionModal != null) protectionModal.SetActive(false);
            if (soldModal != null) soldModal.SetActive(false);
            if (growthModal != null) growthModal.SetActive(false);

            RefreshAll();
        }

        private void OnDestroy()
        {
            if (gm == null) return;
            gm.OnLevelChanged -= OnLevelChanged;
            if (gm.Coins != null) gm.Coins.OnCoinsChanged -= OnCoinsChanged;
            gm.OnProtectionChanged -= OnProtectionChanged;
            gm.OnEnhanceResult -= OnEnhanceResult;
            gm.OnGlobalBestChanged -= OnGlobalBestChanged;
            gm.OnSold -= OnSold;
        }

        // ============================================================
        //  화면 갱신
        // ============================================================
        private void RefreshAll()
        {
            OnLevelChanged(gm.Save.currentLevel);
            OnCoinsChanged(gm.Coins.GetCoins());
            OnProtectionChanged(gm.Save.protectionTickets);
            OnGlobalBestChanged(gm.GlobalBestLevel);
        }

        private void OnLevelChanged(int level)
        {
            var stage = gm.GetCurrentStage();
            if (stage == null) return;

            if (characterImage != null)
            {
                characterImage.sprite = stage.akoSprite;
                characterImage.preserveAspect = true;
                characterImage.color = stage.akoSprite != null
                    ? Color.white : new Color(1, 1, 1, 0.3f);
            }

            if (levelNameText != null)
                levelNameText.text = $"+{level} {stage.akoName}";

            if (successRateText != null)
            {
                // 성장 완료 단계는 확률 표시 안 함
                if (stage.isGrowthCompletion)
                    successRateText.text = "성장 완료";
                else
                    successRateText.text = $"성공률 {stage.successRate:F0}%";
            }

            if (personalBestText != null)
                personalBestText.text = $"내 최고 : +{gm.Save.personalBestLevel}";

            if (sellButton != null)
                sellButton.gameObject.SetActive(gm.CanSellNow());
        }

        private void OnCoinsChanged(int coins)
        {
            if (coinText != null) coinText.text = $"남은 코인 : {coins}";
        }

        private void OnProtectionChanged(int tickets)
        {
            if (protectionText != null) protectionText.text = $"방지권 : {tickets}";
        }

        private void OnGlobalBestChanged(int best)
        {
            if (globalBestText != null) globalBestText.text = $"전체 최고 : +{best}";
        }

        private void OnEnhanceResult(EnhanceResult result)
        {
            switch (result)
            {
                case EnhanceResult.Success:
                    break; // 모달 없음

                case EnhanceResult.GrowthCompleted:
                    if (growthModal != null)
                    {
                        if (growthMessageText != null)
                            growthMessageText.text = "아코 성장 완료!\n\n축하합니다!\n+25 늙코에 도달했어요";
                        growthModal.SetActive(true);
                    }
                    else
                    {
                        // 모달 없으면 그냥 리셋
                        gm.ResetToZero();
                    }
                    break;

                case EnhanceResult.FailedCanUseProtection:
                    if (protectionModal != null)
                    {
                        int cost = gm.GetPendingProtectionCost();
                        if (protectionModalText != null)
                            protectionModalText.text =
                                $"강화에 실패했습니다.\n방지권 {cost}개를 사용하시겠습니까?\n" +
                                $"(보유: {gm.Save.protectionTickets}개)";
                        protectionModal.SetActive(true);
                    }
                    else
                    {
                        gm.ConfirmUseProtection(true);
                    }
                    break;

                case EnhanceResult.FailedReset:
                    if (failModal != null) failModal.SetActive(true);
                    break;
            }
        }

        private void OnSold(AkoStageData soldStage)
        {
            if (soldModal == null || soldMessageText == null) return;

            // 보상 문자열 조합
            var parts = new System.Text.StringBuilder();
            parts.Append("아코를 팔았습니다!\n\n");
            if (soldStage.sellRewardCoins > 0)
                parts.Append($"코인 +{soldStage.sellRewardCoins}");
            if (!string.IsNullOrEmpty(soldStage.sellRewardItem))
            {
                if (soldStage.sellRewardCoins > 0) parts.Append("\n");
                parts.Append($"[{soldStage.sellRewardItem}] 획득");
            }

            soldMessageText.text = parts.ToString();
            soldModal.SetActive(true);
        }

        // ============================================================
        //  버튼
        // ============================================================
        private void OnClickUp()
        {
            if (IsAnyModalOpen()) return;
            gm.TryEnhance();
        }

        private void OnClickSell() => gm.SellCurrentAko();

        private void OnClickShop()
        {
            if (Application.CanStreamedLevelBeLoaded(shopSceneName))
                SceneManager.LoadScene(shopSceneName);
            else
                Debug.LogWarning($"[Ako] '{shopSceneName}' 씬을 Build Profiles에 추가해주세요.");
        }

        private void OnClickProtectionYes()
        {
            if (protectionModal != null) protectionModal.SetActive(false);
            gm.ConfirmUseProtection(true);
        }

        private void OnClickProtectionNo()
        {
            if (protectionModal != null) protectionModal.SetActive(false);
            gm.ConfirmUseProtection(false);
        }

        private void OnClickGrowthClose()
        {
            if (growthModal != null) growthModal.SetActive(false);
            gm.ResetToZero();
        }

        private bool IsAnyModalOpen()
        {
            return (failModal != null && failModal.activeSelf)
                || (protectionModal != null && protectionModal.activeSelf)
                || (soldModal != null && soldModal.activeSelf)
                || (growthModal != null && growthModal.activeSelf);
        }

        private void CloseModal(GameObject modal)
        {
            if (modal != null) modal.SetActive(false);
        }
    }
}
