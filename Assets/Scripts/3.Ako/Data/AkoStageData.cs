using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 한 강화 단계의 데이터.
    /// 
    /// 강화 시도 흐름:
    ///   - 0단계: 코인 1개 차감 후 강화 시도
    ///   - 1단계 이상: 무료로 시도
    /// 
    /// 실패 시 패널티:
    ///   - protectionCost == 0  : 방지권 사용 불가 → 즉시 0단계 리셋
    ///   - protectionCost >= 1  : 보유 방지권이 protectionCost 이상이면 사용 가능
    /// 
    /// 판매:
    ///   - isSellable == true 인 단계에서만 "팔기" 버튼 활성
    ///   - 판매 시 sellRewardCoins 코인 + sellRewardItem 아이템 보상 후 0단계 리셋
    /// </summary>
    [CreateAssetMenu(fileName = "AkoStage_", menuName = "Ako/Stage Data", order = 1)]
    public class AkoStageData : ScriptableObject
    {
        [Header("기본 정보")]
        public int level;
        public string akoName;
        public Sprite akoSprite;

        [Header("강화 확률")]
        [Range(0f, 100f)] public float successRate = 100f;

        [Header("실패 시 방지권")]
        [Tooltip("이 단계에서 다음 단계로 강화 실패 시 방지권을 사용할 수 있는 비용. " +
                 "0이면 방지권 사용 불가(즉시 0단계 리셋). " +
                 "1이면 방지권 1개 소모, 3이면 3개 소모.")]
        [Min(0)] public int protectionCost = 1;

        [Header("판매 보상 (선택)")]
        public bool isSellable = false;
        [Min(0)] public int sellRewardCoins = 0;
        public string sellRewardItem = "";

        [Header("최종 단계 처리")]
        [Tooltip("이 단계에 도달하면 '성장 완료' 메시지를 띄우고 0단계로 자동 초기화. " +
                 "보통 최고 단계(+25 늙코)에만 체크.")]
        public bool isGrowthCompletion = false;
    }
}
