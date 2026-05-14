    using System;
using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 메인 GameManager.Instance와 연동되는 코인 서비스.
    /// 
    /// 메인 GameManager에는 코인 변경 이벤트가 없으므로,
    /// 이 서비스가 직접 OnCoinsChanged를 발행하여 UI를 갱신.
    /// 차감 함수도 없으므로 AddTokens(-amount)로 처리.
    /// </summary>
    public class MainGameManagerCoinService : ICoinService
    {
        public event Action<int> OnCoinsChanged;

        public int GetCoins()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[Ako] GameManager.Instance가 없습니다. 메인 씬에서 시작했는지 확인하세요.");
                return 0;
            }
            return GameManager.Instance.GetTokens();
        }

        public bool SpendCoins(int amount)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[Ako] GameManager.Instance가 없습니다.");
                return false;
            }

            int current = GameManager.Instance.GetTokens();
            if (current < amount) return false;

            // 메인에 SpendTokens가 없으므로 음수 추가로 처리
            GameManager.Instance.AddTokens(-amount);
            OnCoinsChanged?.Invoke(GameManager.Instance.GetTokens());
            return true;
        }

        public void AddCoins(int amount)
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.AddTokens(amount);
            OnCoinsChanged?.Invoke(GameManager.Instance.GetTokens());
        }
    }
}