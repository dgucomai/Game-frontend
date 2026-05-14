#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ako.EditorTools
{
    /// <summary>
    /// 26개 강화 단계 ScriptableObject + Database 에셋을 일괄 생성.
    /// 메뉴: Tools > Ako > Generate All Stage Data
    /// 
    /// 캐릭터 이미지는 Assets/Resources/3.Ako/Sprites/Characters/ 에서
    /// 파일명으로 자동 매칭. 띄어쓰기 차이는 무시하고 매칭함.
    /// </summary>
    public static class AkoStageDataInitializer
    {
        private const string DATA_FOLDER = "Assets/Resources/3.Ako/Data";
        private const string SPRITE_FOLDER = "Assets/Resources/3.Ako/Sprites/Characters";

        // 단계 정보: 이름 / 성공률 / 방지권 비용(0=불가) / 판매가능 / 판매코인 / 판매아이템 / 성장완료
        private static readonly StageInfo[] STAGES =
        {
            new("아기 아코",            100f, 0, false, 0,   "", false),
            new("초딩 아코",             96f, 0, false, 0,   "", false),
            new("모범생 아코",           94f, 0, false, 0,   "", false),
            new("사랑에 빠진 아코",      88f, 0, false, 0,   "", false),
            new("26학번 신입생 아코",    84f, 0, false, 0,   "", false),
            new("카페 알바 아코",        77f, 1, false, 0,   "", false),
            new("컴공 아코",             73f, 1, false, 0,   "", false),
            new("과제 중독 아코",        69f, 1, true,  2,   "", false),
            new("만취 아코",             65f, 1, false, 0,   "", false),
            new("밴드부 리드기타 아코",  58f, 1, false, 0,   "", false),
            new("이병 아코",             54f, 1, true,  10,  "", false),
            new("서커스단 단장 아코",    48f, 1, false, 0,   "", false),
            new("래퍼 아코",             45f, 1, false, 0,   "", false),
            new("컴공 아코 교수",        43f, 1, false, 0,   "", false),
            new("만우절 건대생 아코",    40f, 1, false, 0,   "", false),
            new("카지노 딜러 아코",      36f, 1, true,  20,  "숙취해소제", false),
            new("팝스타 아코",           35f, 3, false, 0,   "", false),
            new("축구선수 아코",         32f, 3, false, 0,   "", false),
            new("스님 아코",             28f, 3, false, 0,   "", false),
            new("캡틴 아코리카",         26f, 3, false, 0,   "", false),
            new("고죠 아코루",           22f, 3, true,  30,  "사이드 메뉴", false),
            new("초사이어인 아코",       70f, 0, false, 0,   "", false),
            new("걸그룹 아코",           50f, 0, false, 0,   "", false),
            new("삼바 아코",             40f, 0, false, 0,   "", false),
            new("헬창 아코",             20f, 0, false, 0,   "", false),
            new("늙코",                   0f, 0, true,  100, "메인 메뉴", true), // 성장 완료
        };

        private struct StageInfo
        {
            public string name;
            public float rate;
            public int protectionCost;
            public bool sellable;
            public int sellCoins;
            public string sellItem;
            public bool growthCompletion;
            public StageInfo(string n, float r, int pc, bool s, int sc, string si, bool gc)
            {
                name = n; rate = r; protectionCost = pc;
                sellable = s; sellCoins = sc; sellItem = si;
                growthCompletion = gc;
            }
        }

        [MenuItem("Tools/Ako/Generate All Stage Data")]
        public static void Generate()
        {
            EnsureFolder(DATA_FOLDER);

            // 스프라이트 폴더 내 모든 파일을 미리 로드해서 띄어쓰기 무시 매칭 준비
            var spriteMap = BuildSpriteMap();
            int missingSprites = 0;

            // 기존 ScriptableObject 파일 삭제 (이름이 바뀐 경우 중복 방지)
            var oldAssets = AssetDatabase.FindAssets("AkoStage_", new[] { DATA_FOLDER });
            foreach (var guid in oldAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }

            var allStages = new List<AkoStageData>();

            for (int i = 0; i < STAGES.Length; i++)
            {
                var info = STAGES[i];

                var asset = ScriptableObject.CreateInstance<AkoStageData>();
                asset.level = i;
                asset.akoName = info.name;
                asset.successRate = info.rate;
                asset.protectionCost = info.protectionCost;
                asset.isSellable = info.sellable;
                asset.sellRewardCoins = info.sellCoins;
                asset.sellRewardItem = info.sellItem;
                asset.isGrowthCompletion = info.growthCompletion;

                // 스프라이트 자동 매칭 (띄어쓰기 무시)
                var sprite = FindSprite(spriteMap, info.name);
                if (sprite != null)
                {
                    asset.akoSprite = sprite;
                }
                else if (!info.growthCompletion || info.name != "늙코")
                {
                    Debug.LogWarning($"[AkoInitializer] 이미지 못 찾음: {info.name}");
                    missingSprites++;
                }
                else
                {
                    // 늙코는 못 찾아도 경고 안 함 (만약 자동매칭에 실수 있을 시)
                    if (sprite == null) missingSprites++;
                }

                // 늙코 매칭 누락 방지 (혹시 위에서 빠졌으면)
                if (asset.akoSprite == null)
                {
                    var s2 = FindSprite(spriteMap, info.name);
                    if (s2 != null) asset.akoSprite = s2;
                }

                string assetPath = $"{DATA_FOLDER}/AkoStage_{i:D2}_{info.name}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                allStages.Add(asset);
            }

            // Database 갱신
            string dbPath = $"{DATA_FOLDER}/AkoDatabase.asset";
            var db = AssetDatabase.LoadAssetAtPath<AkoDatabase>(dbPath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<AkoDatabase>();
                AssetDatabase.CreateAsset(db, dbPath);
            }
            db.stages = allStages;
            EditorUtility.SetDirty(db);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string msg = $"{STAGES.Length}개 단계 데이터 + Database 생성 완료\n경로: {DATA_FOLDER}";
            if (missingSprites > 0)
                msg += $"\n\n⚠️ {missingSprites}개 이미지 매칭 실패.\n" +
                       $"{SPRITE_FOLDER} 안 파일명을 확인해주세요.";

            EditorUtility.DisplayDialog("Ako 데이터 생성", msg, "확인");
        }

        // ============================================================
        //  스프라이트 매칭 (띄어쓰기 무시)
        // ============================================================
        private static Dictionary<string, Sprite> BuildSpriteMap()
        {
            var map = new Dictionary<string, Sprite>();
            if (!AssetDatabase.IsValidFolder(SPRITE_FOLDER))
            {
                Debug.LogWarning($"[AkoInitializer] 스프라이트 폴더 없음: {SPRITE_FOLDER}");
                return map;
            }

            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { SPRITE_FOLDER });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null) continue;

                // 정규화된 이름(공백 제거)을 키로 사용
                string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                string normalized = Normalize(filename);
                if (!map.ContainsKey(normalized))
                    map[normalized] = sprite;
            }
            return map;
        }

        private static Sprite FindSprite(Dictionary<string, Sprite> map, string targetName)
        {
            string normalized = Normalize(targetName);
            return map.TryGetValue(normalized, out var sprite) ? sprite : null;
        }

        private static string Normalize(string s)
        {
            // 모든 공백 제거 + 소문자화 (영문이 섞이면 대응)
            return new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
