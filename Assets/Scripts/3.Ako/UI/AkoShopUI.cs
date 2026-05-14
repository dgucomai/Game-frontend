using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ako
{
    /// <summary>
    /// 상점 화면 UI (3.AkoShop).
    /// 
    /// AkoGameManager가 씬에 없으면 자동 생성하여
    /// 상점 씬을 단독으로 진입해도 동작.
    /// 코인은 AkoCoinHolder를 통해 다른 씬과 공유.
    /// </summary>
    public class AkoShopUI : MonoBehaviour
    {
        [Header("방지권 구매")]
        public Button buyProtection1;
        public Button buyProtection3;
        public Button buyProtection10;

        [Header("워프권 사용")]
        public Button useWarp5;
        public Button useWarp7;
        public Button useWarp13;

        [Header("기타")]
        public Button backButton;
        public Text coinText;
        public Text protectionText;

        [Header("씬 이름")]
        public string gameSceneName = "3.Ako";

        [Header("매니저 자동 생성에 필요한 Database")]
        [Tooltip("상점 씬 단독 진입 시 GameManager를 자동 생성할 때 사용")]
        public AkoDatabase databaseFallback;

        private AkoGameManager gm;

        private void Start()
        {
            // GameManager가 없으면 자동 생성
            gm = AkoGameManager.Instance;
            if (gm == null)
            {
                gm = EnsureGameManager();
            }

            if (gm == null)
            {
                Debug.LogError("[AkoShopUI] AkoGameManager 생성 실패. " +
                               "Database가 연결되어 있는지 확인하세요.");
                return;
            }

            // 버튼
            if (buyProtection1 != null)
                buyProtection1.onClick.AddListener(() => gm.BuyProtectionTickets(1, 1));
            if (buyProtection3 != null)
                buyProtection3.onClick.AddListener(() => gm.BuyProtectionTickets(3, 2));
            if (buyProtection10 != null)
                buyProtection10.onClick.AddListener(() => gm.BuyProtectionTickets(10, 7));

            if (useWarp5 != null)
                useWarp5.onClick.AddListener(() => gm.UseWarpTicket(5, 3));
            if (useWarp7 != null)
                useWarp7.onClick.AddListener(() => gm.UseWarpTicket(7, 5));
            if (useWarp13 != null)
                useWarp13.onClick.AddListener(() => gm.UseWarpTicket(13, 9));

            if (backButton != null)
                backButton.onClick.AddListener(OnClickBack);

            // 이벤트 구독
            gm.Coins.OnCoinsChanged += UpdateCoinText;
            gm.OnProtectionChanged += UpdateProtectionText;

            // 초기값
            UpdateCoinText(gm.Coins.GetCoins());
            UpdateProtectionText(gm.Save.protectionTickets);
        }

        private void OnDestroy()
        {
            if (gm == null) return;
            if (gm.Coins != null) gm.Coins.OnCoinsChanged -= UpdateCoinText;
            gm.OnProtectionChanged -= UpdateProtectionText;
        }

        private void UpdateCoinText(int coins)
        {
            if (coinText != null) coinText.text = $"보유 코인 : {coins}";
        }

        private void UpdateProtectionText(int tickets)
        {
            if (protectionText != null) protectionText.text = $"방지권 : {tickets}";
        }

        private void OnClickBack()
        {
            if (Application.CanStreamedLevelBeLoaded(gameSceneName))
                SceneManager.LoadScene(gameSceneName);
        }

        /// <summary>
        /// 상점 씬을 단독으로 시작한 경우 GameManager를 자동 생성.
        /// Resources에서 Database를 찾아 자동 연결 시도.
        /// </summary>
        private AkoGameManager EnsureGameManager()
        {
            var go = new GameObject("AkoGameManager");
            var manager = go.AddComponent<AkoGameManager>();

            // Database 자동 연결 (Inspector 슬롯 fallback → Resources 검색)
            var db = databaseFallback;
            if (db == null)
            {
                // Resources에서 자동 로드 시도
                db = Resources.Load<AkoDatabase>("3.Ako/Data/AkoDatabase");
            }

            if (db != null)
            {
                // private 필드라 SerializedObject로 접근하지 않고 reflection 사용
                var field = typeof(AkoGameManager)
                    .GetField("database",
                              System.Reflection.BindingFlags.NonPublic |
                              System.Reflection.BindingFlags.Instance);
                if (field != null) field.SetValue(manager, db);
            }
            else
            {
                Debug.LogWarning("[AkoShopUI] Database를 자동 로드할 수 없습니다. " +
                                 "databaseFallback 슬롯에 직접 연결하거나 " +
                                 "Database를 Resources/3.Ako/Data/ 안에 두세요.");
            }

            // Awake가 호출되도록 한 프레임 대기는 불필요 (AddComponent 시 즉시 호출됨)
            return manager;
        }
    }
}
