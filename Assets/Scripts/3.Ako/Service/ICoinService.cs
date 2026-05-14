using System;
using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 메인 게임 서비스와의 코인 데이터 교환 인터페이스.
    /// 모든 씬(메인 게임, 상점)이 같은 코인 풀에 접근하기 위해 사용.
    /// </summary>
    public interface ICoinService
    {
        /// <summary>현재 보유 코인.</summary>
        int GetCoins();

        /// <summary>코인 차감. 부족하면 false.</summary>
        bool SpendCoins(int amount);

        /// <summary>코인 추가 (메인에서 보상 지급 등).</summary>
        void AddCoins(int amount);

        /// <summary>코인 변동 이벤트.</summary>
        event Action<int> OnCoinsChanged;
    }

    /// <summary>
    /// 코인 서비스의 전역 보관소.
    /// 
    /// 모든 씬(3.Ako, 3.AkoShop)에서 AkoCoinHolder.Service로 접근.
    /// 같은 인스턴스를 공유하므로 코인 값이 씬 간에 일관됨.
    /// 
    /// 메인 연동 시 교체 방법:
    ///   AkoCoinHolder.SetService(new MainCoinService());
    /// </summary>
    public static class AkoCoinHolder
    {
        private static ICoinService service;

        public static ICoinService Service
        {
            get
            {
                if (service == null)
                    service = new MockCoinService();
                return service;
            }
        }

        public static void SetService(ICoinService newService)
        {
            service = newService;
        }
    }

    /// <summary>
    /// 임시 코인 구현. 메인 연동 전까지 사용.
    /// PlayerPrefs에 저장하여 씬 이동/재시작 후에도 값 유지.
    /// </summary>
    public class MockCoinService : ICoinService
    {
        private const string COIN_KEY = "Ako_MockCoins";
        private const int INITIAL_COINS = 10;
        private int coins;

        public event Action<int> OnCoinsChanged;

        public MockCoinService()
        {
            if (PlayerPrefs.HasKey(COIN_KEY))
                coins = PlayerPrefs.GetInt(COIN_KEY);
            else
            {
                coins = INITIAL_COINS;
                PlayerPrefs.SetInt(COIN_KEY, coins);
            }
        }

        public int GetCoins() => coins;

        public bool SpendCoins(int amount)
        {
            if (coins < amount) return false;
            coins -= amount;
            PlayerPrefs.SetInt(COIN_KEY, coins);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(coins);
            return true;
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            PlayerPrefs.SetInt(COIN_KEY, coins);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(coins);
        }

        /// <summary>디버그용 리셋.</summary>
        public void Reset()
        {
            coins = INITIAL_COINS;
            PlayerPrefs.SetInt(COIN_KEY, coins);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(coins);
        }
    }
}
