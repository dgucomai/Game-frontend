namespace Ako
{
    /// <summary>
    /// 강화 시도의 결과 종류.
    /// </summary>
    public enum EnhanceResult
    {
        /// <summary>강화 성공. 다음 단계로 진행.</summary>
        Success,

        /// <summary>강화 실패. 방지권 사용 가능 → 사용 여부를 사용자에게 물어봐야 함.</summary>
        FailedCanUseProtection,

        /// <summary>강화 실패. 방지권 사용 불가(또는 부족) → 0단계로 즉시 초기화됨.</summary>
        FailedReset,

        /// <summary>강화 성공하여 최종 단계(+25 늙코)에 도달. 성장 완료 → 곧 0단계로 초기화됨.</summary>
        GrowthCompleted
    }
}
