using UnityEngine;

namespace Ako
{
    public static class AkoSaveManager
    {
        private const string SAVE_KEY = "AkoGameSave_v1";

        public static void Save(AkoSaveData data)
        {
            if (data == null) return;
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public static AkoSaveData Load()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY)) return new AkoSaveData();
            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var data = JsonUtility.FromJson<AkoSaveData>(json);
                return data ?? new AkoSaveData();
            }
            catch
            {
                Debug.LogWarning("[Ako] 저장 데이터 손상. 새로 시작.");
                return new AkoSaveData();
            }
        }

        public static void Clear() => PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}
