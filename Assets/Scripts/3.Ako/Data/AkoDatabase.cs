using System.Collections.Generic;
using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 모든 강화 단계를 모아둔 데이터베이스.
    /// </summary>
    [CreateAssetMenu(fileName = "AkoDatabase", menuName = "Ako/Database", order = 0)]
    public class AkoDatabase : ScriptableObject
    {
        [Tooltip("0단계부터 마지막 단계까지 순서대로 등록")]
        public List<AkoStageData> stages = new List<AkoStageData>();

        public int MaxLevel => stages.Count - 1;

        public AkoStageData GetStage(int level)
        {
            if (level < 0 || level >= stages.Count) return null;
            return stages[level];
        }
    }
}
