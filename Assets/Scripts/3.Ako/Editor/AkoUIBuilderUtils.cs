#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ako.EditorTools
{
    internal enum FontKind { Body, Display }

    internal static class AkoUIBuilderUtils
    {
        public static readonly Color BG_ORANGE      = new Color(0.99f, 0.55f, 0.32f, 1f);
        public static readonly Color BUTTON_CREAM   = new Color(0.99f, 0.96f, 0.85f, 1f);
        public static readonly Color CARD_WHITE     = new Color(1f, 1f, 1f, 0.95f);
        public static readonly Color OVERLAY_DARK   = new Color(0f, 0f, 0f, 0.6f);
        public static readonly Color TEXT_DARK      = new Color(0.15f, 0.15f, 0.15f, 1f);
        public static readonly Color TEXT_WHITE     = Color.white;

        public enum AnchorPreset
        {
            TopLeft, TopCenter, TopRight,
            MiddleLeft, MiddleCenter, MiddleRight,
            BottomLeft, BottomCenter, BottomRight
        }

        public static (Vector2 anchor, Vector2 pivot) GetAnchorPivot(AnchorPreset p)
        {
            switch (p)
            {
                case AnchorPreset.TopLeft:      return (new Vector2(0, 1),     new Vector2(0, 1));
                case AnchorPreset.TopCenter:    return (new Vector2(0.5f, 1),  new Vector2(0.5f, 1));
                case AnchorPreset.TopRight:     return (new Vector2(1, 1),     new Vector2(1, 1));
                case AnchorPreset.MiddleLeft:   return (new Vector2(0, 0.5f),  new Vector2(0, 0.5f));
                case AnchorPreset.MiddleCenter: return (new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                case AnchorPreset.MiddleRight:  return (new Vector2(1, 0.5f),  new Vector2(1, 0.5f));
                case AnchorPreset.BottomLeft:   return (new Vector2(0, 0),     new Vector2(0, 0));
                case AnchorPreset.BottomCenter: return (new Vector2(0.5f, 0), new Vector2(0.5f, 0));
                case AnchorPreset.BottomRight:  return (new Vector2(1, 0),     new Vector2(1, 0));
            }
            return (new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }

        public static void SetAnchored(RectTransform rt, AnchorPreset preset, Vector2 pos, Vector2 size)
        {
            var (a, p) = GetAnchorPivot(preset);
            rt.anchorMin = a;
            rt.anchorMax = a;
            rt.pivot = p;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        public static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static GameObject CreateCanvas(string name)
        {
            var go = new GameObject(name,
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            return go;
        }

        public static void EnsureEventSystem()
        {
#if UNITY_2023_1_OR_NEWER
            var existing = Object.FindFirstObjectByType<EventSystem>();
#else
            var existing = Object.FindObjectOfType<EventSystem>();
#endif
            if (existing != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        public static Image CreateImage(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            return img;
        }

        public static Image CreateImageWithSprite(Transform parent, string name, Sprite sprite)
        {
            var img = CreateImage(parent, name, Color.white);
            img.sprite = sprite;
            img.preserveAspect = true;
            if (sprite == null) img.color = new Color(1, 1, 1, 0.3f);
            return img;
        }

        /// <summary>
        /// 텍스트 생성. 기본 폰트는 Body. Display 지정 시 디스플레이 폰트 사용.
        /// </summary>
        public static Text CreateText(Transform parent, string name, string content,
                                       int fontSize, FontStyle style, Color color,
                                       FontKind fontKind = FontKind.Body)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.text = content;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.font = LoadFont(fontKind);
            return t;
        }

        /// <summary>
        /// 버튼 생성. 라벨 폰트 종류 지정 가능.
        /// </summary>
        public static Button CreateButton(Transform parent, string name, string label, int fontSize,
                                           Color? bgColor = null, FontKind fontKind = FontKind.Body)
        {
            var go = new GameObject(name,
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = bgColor ?? BUTTON_CREAM;
            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;

            var colors = btn.colors;
            Color baseColor = bgColor ?? BUTTON_CREAM;
            colors.normalColor = baseColor;
            colors.highlightedColor = new Color(baseColor.r * 0.95f, baseColor.g * 0.95f, baseColor.b * 0.95f, baseColor.a);
            colors.pressedColor = new Color(baseColor.r * 0.85f, baseColor.g * 0.85f, baseColor.b * 0.85f, baseColor.a);
            btn.colors = colors;

            var text = CreateText(go.transform, "Text", label, fontSize, FontStyle.Bold, TEXT_DARK, fontKind);
            StretchFull(text.rectTransform);
            return btn;
        }

        public static RectTransform CreatePanel(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        public static Sprite LoadSprite(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        /// <summary>
        /// 에디터에서 폰트를 로드. Resources 폴더에 있어야 함.
        /// </summary>
        private static Font LoadFont(FontKind kind)
        {
            string path = kind == FontKind.Display
                ? "Assets/Resources/3.Ako/Fonts/BagelFatOne-Regular.ttf"
                : "Assets/Resources/3.Ako/Fonts/HakgyoansimDoongDoongmiso-B.ttf";

            var font = AssetDatabase.LoadAssetAtPath<Font>(path);
            if (font == null)
            {
                Debug.LogWarning($"[AkoUIBuilder] 폰트 못 찾음: {path}\n" +
                                 "Resources/3.Ako/Fonts/ 폴더에 .ttf를 넣어주세요. " +
                                 "기본 폰트로 대체합니다.");
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return font;
        }
    }
}
#endif
