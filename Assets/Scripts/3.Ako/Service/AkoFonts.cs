using UnityEngine;

namespace Ako
{
    /// <summary>
    /// 게임에 사용되는 폰트의 전역 보관소.
    /// 
    /// - DisplayFont: 타이틀/START/UP 같은 큰 디스플레이용 (Bagel Fat One)
    /// - BodyFont:    일반 텍스트/버튼/메시지 (학교안심 둥근미소 Bold)
    /// 
    /// Resources/3.Ako/Fonts/ 에서 자동 로드. 못 찾으면 기본 폰트 사용.
    /// </summary>
    public static class AkoFonts
    {
        private static Font displayFont;
        private static Font bodyFont;
        private static bool loaded = false;

        public static Font DisplayFont
        {
            get { EnsureLoaded(); return displayFont; }
        }

        public static Font BodyFont
        {
            get { EnsureLoaded(); return bodyFont; }
        }

        private static void EnsureLoaded()
        {
            if (loaded) return;
            loaded = true;

            displayFont = Resources.Load<Font>("3.Ako/Fonts/BagelFatOne-Regular");
            bodyFont = Resources.Load<Font>("3.Ako/Fonts/HakgyoansimDoongDoongmiso-B");

            if (displayFont == null)
            {
                Debug.LogWarning("[AkoFonts] DisplayFont(BagelFatOne-Regular)을 못 찾았습니다. " +
                                 "Resources/3.Ako/Fonts/ 안에 .ttf 파일이 있는지 확인하세요.");
                displayFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            if (bodyFont == null)
            {
                Debug.LogWarning("[AkoFonts] BodyFont(HakgyoansimDoongDoongmiso-B)을 못 찾았습니다.");
                bodyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }
    }
}
