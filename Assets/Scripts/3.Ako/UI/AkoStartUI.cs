using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Ako
{
    /// <summary>
    /// 시작 화면 UI (3.AkoStart).
    /// </summary>
    public class AkoStartUI : MonoBehaviour
    {
        [Header("버튼")]
        public Button startButton;

        [Header("씬 이름")]
        public string gameSceneName = "3.Ako";

        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnClickStart);
        }

        private void OnClickStart()
        {
            if (Application.CanStreamedLevelBeLoaded(gameSceneName))
                SceneManager.LoadScene(gameSceneName);
            else
                Debug.LogWarning($"[AkoStartUI] '{gameSceneName}' 씬을 Build Profiles에 추가해주세요.");
        }
    }
}
