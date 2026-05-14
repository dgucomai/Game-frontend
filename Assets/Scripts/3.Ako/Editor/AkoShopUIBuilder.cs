#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Ako.EditorTools.AkoUIBuilderUtils;

namespace Ako.EditorTools
{
    public static class AkoShopUIBuilder
    {
        [MenuItem("Tools/Ako/Build Shop UI")]
        public static void Build()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                EditorUtility.DisplayDialog("오류", "씬이 열려있지 않습니다.", "확인");
                return;
            }

            var existing = GameObject.Find("AkoShopCanvas");
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("기존 UI 발견",
                    "이미 'AkoShopCanvas'가 있습니다.\n삭제하고 새로 만들까요?",
                    "예, 새로 만들기", "취소"))
                    return;
                Object.DestroyImmediate(existing);
            }

            var canvasGO = CreateCanvas("AkoShopCanvas");
            var canvasT = canvasGO.transform;
            EnsureEventSystem();

            var bg = CreateImage(canvasT, "Background", BG_ORANGE);
            StretchFull(bg.rectTransform);

            // 타이틀 (시안 상으로는 같은 "아코 키우기" - 시작/메인과 통일감 위해 Display)
            var title = CreateText(canvasT, "TitleText", "아코 키우기",
                                    70, FontStyle.Bold, TEXT_WHITE, FontKind.Display);
            SetAnchored(title.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -110), new Vector2(800, 110));

            var subtitle = CreateText(canvasT, "SubtitleText", "WELCOME TO SHOP",
                                       50, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(subtitle.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -210), new Vector2(800, 80));

            var backBtn = CreateButton(canvasT, "BackButton", "BACK", 55);
            SetAnchored(backBtn.GetComponent<RectTransform>(), AnchorPreset.TopLeft,
                        new Vector2(50, -70), new Vector2(200, 130));

            // 상점 아주머니
            var shopkeeperSprite = LoadSprite("Assets/Resources/3.Ako/Sprites/UI/shopkeeper_ako.png");
            var shopkeeper = CreateImageWithSprite(canvasT, "ShopkeeperAko", shopkeeperSprite);
            SetAnchored(shopkeeper.rectTransform, AnchorPreset.TopRight,
                        new Vector2(-40, -50), new Vector2(360, 400));

            // 좌측 [방지권 구매]
            var leftHeader = CreateText(canvasT, "LeftHeader", "[ 방지권 구매 ]",
                                         50, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(leftHeader.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(-270, 550), new Vector2(500, 80));

            var btnP1  = CreateShopItem(canvasT, "BuyProtection1Panel",  "방지권",     "1 COIN", -270, 280);
            var btnP3  = CreateShopItem(canvasT, "BuyProtection3Panel",  "방지권 x3",  "2 COIN", -270, 0);
            var btnP10 = CreateShopItem(canvasT, "BuyProtection10Panel", "방지권 x10", "7 COIN", -270, -280);

            // 우측 [아이템 구매]
            var rightHeader = CreateText(canvasT, "RightHeader", "[ 아이템 구매 ]",
                                          50, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(rightHeader.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(270, 550), new Vector2(500, 80));

            var btnW5  = CreateShopItem(canvasT, "UseWarp5Panel",  "워프권 5강",  "3 COIN", 270, 280);
            var btnW7  = CreateShopItem(canvasT, "UseWarp7Panel",  "워프권 7강",  "5 COIN", 270, 0);
            var btnW13 = CreateShopItem(canvasT, "UseWarp13Panel", "워프권 13강", "9 COIN", 270, -280);

            // 구분선
            var divider = CreateImage(canvasT, "Divider", new Color(1, 1, 1, 0.4f));
            SetAnchored(divider.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, 100), new Vector2(4, 900));

            // 하단
            var coinText = CreateText(canvasT, "CoinText", "보유 코인 : 10",
                                       50, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(coinText.rectTransform, AnchorPreset.BottomLeft,
                        new Vector2(60, 80), new Vector2(500, 80));
            coinText.alignment = TextAnchor.MiddleLeft;

            var protectionText = CreateText(canvasT, "ProtectionText", "방지권 : 0",
                                             50, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(protectionText.rectTransform, AnchorPreset.BottomRight,
                        new Vector2(-60, 80), new Vector2(500, 80));
            protectionText.alignment = TextAnchor.MiddleRight;

            var ui = canvasGO.AddComponent<AkoShopUI>();
            ui.buyProtection1 = btnP1;
            ui.buyProtection3 = btnP3;
            ui.buyProtection10 = btnP10;
            ui.useWarp5 = btnW5;
            ui.useWarp7 = btnW7;
            ui.useWarp13 = btnW13;
            ui.backButton = backBtn;
            ui.coinText = coinText;
            ui.protectionText = protectionText;

            const string dbPath = "Assets/Resources/3.Ako/Data/AkoDatabase.asset";
            var db = AssetDatabase.LoadAssetAtPath<AkoDatabase>(dbPath);
            if (db != null) ui.databaseFallback = db;

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeGameObject = canvasGO;

            EditorUtility.DisplayDialog(
                "상점 UI 생성 완료",
                "AkoShopCanvas가 만들어졌습니다.\n\nCtrl+S로 씬을 '3.AkoShop'으로 저장하세요.",
                "확인");
        }

        private static Button CreateShopItem(Transform parent, string panelName,
                                              string label, string price, float x, float y)
        {
            var card = CreateImage(parent, panelName, new Color(1, 1, 1, 0.15f));
            SetAnchored(card.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(x, y), new Vector2(440, 220));

            var labelText = CreateText(card.transform, "Label", label,
                                        45, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(labelText.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -15), new Vector2(400, 60));

            var priceText = CreateText(card.transform, "Price", price,
                                        40, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(priceText.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, 5), new Vector2(400, 60));

            var btn = CreateButton(card.transform, "Button", "구매", 40);
            SetAnchored(btn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(0, 15), new Vector2(360, 70));

            return btn;
        }
    }
}
#endif
