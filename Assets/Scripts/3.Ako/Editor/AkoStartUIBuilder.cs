#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Ako.EditorTools.AkoUIBuilderUtils;

namespace Ako.EditorTools
{
    public static class AkoStartUIBuilder
    {
        [MenuItem("Tools/Ako/Build Start UI")]
        public static void Build()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                EditorUtility.DisplayDialog("오류", "씬이 열려있지 않습니다.", "확인");
                return;
            }

            var existing = GameObject.Find("AkoStartCanvas");
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("기존 UI 발견",
                    "이미 'AkoStartCanvas'가 있습니다.\n삭제하고 새로 만들까요?",
                    "예, 새로 만들기", "취소"))
                    return;
                Object.DestroyImmediate(existing);
            }

            var canvasGO = CreateCanvas("AkoStartCanvas");
            var canvasT = canvasGO.transform;
            EnsureEventSystem();

            var bg = CreateImage(canvasT, "Background", BG_ORANGE);
            StretchFull(bg.rectTransform);

            // 타이틀 - Display 폰트
            var title = CreateText(canvasT, "TitleText", "아코 키우기",
                                    130, FontStyle.Bold, TEXT_WHITE, FontKind.Display);
            SetAnchored(title.rectTransform, AnchorPreset.TopCenter,
                        new Vector2(0, -280), new Vector2(900, 200));

            // 표지 아코
            var coverSprite = LoadSprite("Assets/Resources/3.Ako/Sprites/UI/cover_ako.png");
            var character = CreateImageWithSprite(canvasT, "CoverAko", coverSprite);
            SetAnchored(character.rectTransform, AnchorPreset.MiddleCenter,
                        new Vector2(0, 80), new Vector2(800, 800));

            // START 버튼 - Display 폰트
            var startBtn = CreateButton(canvasT, "StartButton", "START", 130, null, FontKind.Display);
            SetAnchored(startBtn.GetComponent<RectTransform>(), AnchorPreset.BottomCenter,
                        new Vector2(0, 380), new Vector2(550, 220));

            var ui = canvasGO.AddComponent<AkoStartUI>();
            ui.startButton = startBtn;

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeGameObject = canvasGO;

            EditorUtility.DisplayDialog(
                "시작 UI 생성 완료",
                "AkoStartCanvas가 만들어졌습니다.\n\nCtrl+S로 씬을 '3.AkoStart'로 저장하세요.",
                "확인");
        }
    }
}
#endif
