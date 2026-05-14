#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Ako.EditorTools.AkoUIBuilderUtils;

namespace Ako.EditorTools
{
    public static class AkoMainUIBuilder
    {
        [MenuItem("Tools/Ako/Build Main UI")]
        public static void Build()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                EditorUtility.DisplayDialog("오류", "씬이 열려있지 않습니다.", "확인");
                return;
            }

            var existing = GameObject.Find("AkoMainCanvas");
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("기존 UI 발견",
                    "이미 'AkoMainCanvas'가 있습니다.\n삭제하고 새로 만들까요?",
                    "예, 새로 만들기", "취소"))
                    return;
                Object.DestroyImmediate(existing);
            }

            var canvasGO = CreateCanvas("AkoMainCanvas");
            var canvasT = canvasGO.transform;
            EnsureEventSystem();

            var bg = CreateImage(canvasT, "Background", BG_ORANGE);
            StretchFull(bg.rectTransform);

            // [영역 1] 상단바: 타이틀 + 상점
            var title = CreateText(canvasT, "TitleText", "아코 키우기",
                                    85, FontStyle.Bold, TEXT_WHITE, FontKind.Display);
            SetAnchored(title.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -140), new Vector2(800, 120));

            var shopBtn = CreateButton(canvasT, "ShopButton", "상점", 50);
            SetAnchored(shopBtn.GetComponent<RectTransform>(), AnchorPreset.TopRight,
                        new Vector2(-50, -80), new Vector2(180, 110));

            // [영역 2] 기록
            var personalBest = CreateText(canvasT, "PersonalBestText", "내 최고 : +0",
                                           45, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(personalBest.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -310), new Vector2(900, 60));

            var globalBest = CreateText(canvasT, "GlobalBestText", "전체 최고 : +0",
                                         45, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(globalBest.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -380), new Vector2(900, 60));

            // [영역 3] 캐릭터 카드
            var charCard = CreateImage(canvasT, "CharacterCard", CARD_WHITE);
            SetAnchored(charCard.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, 235), new Vector2(620, 620));

            var charImage = CreateImage(charCard.transform, "CharacterImage", Color.white);
            charImage.preserveAspect = true;
            SetAnchored(charImage.rectTransform, AnchorPreset.MiddleCenter,
                        Vector2.zero, new Vector2(560, 560));

            // [영역 4] 단계/이름 + 성공률
            var levelNameText = CreateText(canvasT, "LevelNameText", "+0 아기 아코",
                                            75, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(levelNameText.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, -150), new Vector2(900, 90));

            var successRateText = CreateText(canvasT, "SuccessRateText", "성공률 100%",
                                              50, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(successRateText.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, -240), new Vector2(900, 70));

            // [영역 5] 액션
            // UP! 버튼 - Display 폰트
            var upBtn = CreateButton(canvasT, "UpButton", "UP!", 110, CARD_WHITE, FontKind.Display);
            SetAnchored(upBtn.GetComponent<RectTransform>(), AnchorPreset.MiddleCenter,
                        new Vector2(0, -500), new Vector2(300, 300));

            var sellBtn = CreateButton(canvasT, "SellButton", "팔기", 55);
            SetAnchored(sellBtn.GetComponent<RectTransform>(), AnchorPreset.MiddleCenter,
                        new Vector2(370, -500), new Vector2(200, 200));
            sellBtn.gameObject.SetActive(false);

            // [영역 6] 하단바
            var protectionText = CreateText(canvasT, "ProtectionText", "방지권 : 0",
                                             55, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(protectionText.rectTransform, AnchorPreset.BottomLeft,
                        new Vector2(60, 80), new Vector2(500, 80));
            protectionText.alignment = TextAnchor.MiddleLeft;

            var coinText = CreateText(canvasT, "CoinText", "남은 코인 : 10",
                                       55, FontStyle.Normal, TEXT_WHITE);
            SetAnchored(coinText.rectTransform, AnchorPreset.BottomRight,
                        new Vector2(-60, 80), new Vector2(500, 80));
            coinText.alignment = TextAnchor.MiddleRight;

            // 모달들
            var crySprite = LoadSprite("Assets/Resources/3.Ako/Sprites/UI/crying_ako.png");
            var (failModal, failClose) = CreateFailModal(canvasT, "FailModal", crySprite);
            var (protectionModal, protYesBtn, protNoBtn, protMsgText) =
                CreateProtectionModal(canvasT, "ProtectionModal");
            var (soldModal, soldClose, soldMsg) = CreateSimpleModal(
                canvasT, "SoldModal", "판매 완료!", "확인", new Color(0.95f, 0.75f, 0.3f, 1f));
            var (growthModal, growthClose, growthMsg) = CreateSimpleModal(
                canvasT, "GrowthModal", "아코 성장 완료!", "다시 시작", new Color(0.4f, 0.7f, 0.95f, 1f));

            // 컴포넌트 연결
            var ui = canvasGO.AddComponent<AkoMainUI>();
            ui.characterImage = charImage;
            ui.levelNameText = levelNameText;
            ui.successRateText = successRateText;
            ui.personalBestText = personalBest;
            ui.globalBestText = globalBest;
            ui.protectionText = protectionText;
            ui.coinText = coinText;
            ui.upButton = upBtn;
            ui.sellButton = sellBtn;
            ui.shopButton = shopBtn;
            ui.failModal = failModal;
            ui.failModalCloseButton = failClose;
            ui.protectionModal = protectionModal;
            ui.protectionYesButton = protYesBtn;
            ui.protectionNoButton = protNoBtn;
            ui.protectionModalText = protMsgText;
            ui.soldModal = soldModal;
            ui.soldModalCloseButton = soldClose;
            ui.soldMessageText = soldMsg;
            ui.growthModal = growthModal;
            ui.growthCloseButton = growthClose;
            ui.growthMessageText = growthMsg;

            EnsureGameManager();
            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeGameObject = canvasGO;

            EditorUtility.DisplayDialog(
                "메인 UI 생성 완료",
                "AkoMainCanvas가 만들어졌습니다.\n\nCtrl+S로 씬을 '3.Ako'로 저장하세요.",
                "확인");
        }

        private static (GameObject modal, Button closeBtn) CreateFailModal(
            Transform parent, string name, Sprite akoSprite)
        {
            var modal = CreatePanel(parent, name);
            StretchFull(modal);
            var overlay = CreateImage(modal.transform, "Overlay", OVERLAY_DARK);
            StretchFull(overlay.rectTransform);
            var content = CreatePanel(modal.transform, "Content");
            SetAnchored(content, AnchorPreset.MiddleCenter,
                        Vector2.zero, new Vector2(900, 1200));

            var akoImage = CreateImageWithSprite(content.transform, "AkoImage", akoSprite);
            SetAnchored(akoImage.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -50), new Vector2(600, 600));

            var msgText = CreateText(content.transform, "Message",
                "강화에 실패하여\n아코가 슬픔에 빠졌습니다...",
                55, FontStyle.Bold, TEXT_WHITE);
            SetAnchored(msgText.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, -150), new Vector2(800, 200));

            var closeBtn = CreateButton(content.transform, "CloseButton", "확인", 60,
                                         new Color(0.95f, 0.3f, 0.3f, 1f));
            SetAnchored(closeBtn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(0, 50), new Vector2(400, 130));

            modal.gameObject.SetActive(false);
            return (modal.gameObject, closeBtn);
        }

        private static (GameObject modal, Button yesBtn, Button noBtn, Text msgText)
            CreateProtectionModal(Transform parent, string name)
        {
            var modal = CreatePanel(parent, name);
            StretchFull(modal);
            var overlay = CreateImage(modal.transform, "Overlay", OVERLAY_DARK);
            StretchFull(overlay.rectTransform);
            var content = CreateImage(modal.transform, "Content", CARD_WHITE);
            SetAnchored(content.rectTransform, AnchorPreset.MiddleCenter,
                        Vector2.zero, new Vector2(900, 600));

            var msgText = CreateText(content.transform, "Message",
                "강화에 실패했습니다.\n방지권을 사용하시겠습니까?",
                48, FontStyle.Bold, TEXT_DARK);
            SetAnchored(msgText.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -80), new Vector2(800, 280));

            var yesBtn = CreateButton(content.transform, "YesButton", "Yes", 60,
                                       new Color(0.4f, 0.8f, 0.4f, 1f));
            SetAnchored(yesBtn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(-180, 60), new Vector2(280, 130));

            var noBtn = CreateButton(content.transform, "NoButton", "No", 60,
                                      new Color(0.95f, 0.3f, 0.3f, 1f));
            SetAnchored(noBtn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(180, 60), new Vector2(280, 130));

            modal.gameObject.SetActive(false);
            return (modal.gameObject, yesBtn, noBtn, msgText);
        }

        private static (GameObject modal, Button closeBtn, Text msgText)
            CreateSimpleModal(Transform parent, string name, string initMsg, string btnLabel, Color btnColor)
        {
            var modal = CreatePanel(parent, name);
            StretchFull(modal);
            var overlay = CreateImage(modal.transform, "Overlay", OVERLAY_DARK);
            StretchFull(overlay.rectTransform);
            var content = CreateImage(modal.transform, "Content", CARD_WHITE);
            SetAnchored(content.rectTransform, AnchorPreset.MiddleCenter,
                        Vector2.zero, new Vector2(900, 650));

            var msgText = CreateText(content.transform, "Message", initMsg,
                                      55, FontStyle.Bold, TEXT_DARK);
            SetAnchored(msgText.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, 60), new Vector2(800, 400));

            var closeBtn = CreateButton(content.transform, "CloseButton", btnLabel, 55, btnColor);
            SetAnchored(closeBtn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(0, 60), new Vector2(400, 130));

            modal.gameObject.SetActive(false);
            return (modal.gameObject, closeBtn, msgText);
        }

        private static void EnsureGameManager()
        {
#if UNITY_2023_1_OR_NEWER
            var gm = Object.FindFirstObjectByType<AkoGameManager>();
#else
            var gm = Object.FindObjectOfType<AkoGameManager>();
#endif
            if (gm == null)
            {
                var go = new GameObject("AkoGameManager");
                gm = go.AddComponent<AkoGameManager>();
            }

            const string dbPath = "Assets/Resources/3.Ako/Data/AkoDatabase.asset";
            var db = AssetDatabase.LoadAssetAtPath<AkoDatabase>(dbPath);
            if (db != null)
            {
                var so = new SerializedObject(gm);
                var dbProp = so.FindProperty("database");
                if (dbProp != null && dbProp.objectReferenceValue == null)
                {
                    dbProp.objectReferenceValue = db;
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogWarning($"[AkoMainUIBuilder] Database 못 찾음: {dbPath}");
            }
        }
    }
}
#endif
