using System;
using UnityEngine;

namespace Ako
{
    public interface IAkoLeaderboardService
    {
        void GetGlobalBest(Action<int> onResult);
        void SubmitMyBest(int level);
    }

    public class MockLeaderboardService : IAkoLeaderboardService
    {
        public void GetGlobalBest(Action<int> onResult)
        {
            onResult?.Invoke(20);
        }

        public void SubmitMyBest(int level)
        {
            Debug.Log($"[MockLeaderboard] 기록 제출됨: {level}단계");
        }
    }
}
