using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    string apiUrl = "https://donggukcomai.shop/api";
    int lastWidth, lastHeight;

    // --- WebGL URL 매개변수 읽기용 JS 연결 ---
    [DllImport("__Internal")]
    private static extern string GetTableIdFromURL();

    [Header("Player Data")]
    public string nickname;
    public string currentTableId;
    public int tokens;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitGameSession();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                SetResolution();
            }
        }
    }

    void InitGameSession()
    {
        // 1. URL에서 tableId 가져오기
        #if !UNITY_EDITOR && UNITY_WEBGL
        currentTableId = GetTableIdFromURL();
        #else
        currentTableId = "abc123";
        #endif

        // 2. 서버에서 토큰 정보 가져오기 시작
        if (!string.IsNullOrEmpty(currentTableId))
        {
            StartCoroutine(LoadTokenRoutine());
        }
    }

    public IEnumerator LoadTokenRoutine()
    {
        Debug.Log("서버에 토큰 조회 요청 중...");

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + "/menus"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                Debug.Log(request.downloadHandler.text);
                tokens = 5000; // 임시로 고정값 할당
            }
        }
    }

    public void LoadScene(int index)
    {
        if (index < 0 || index > 4) return;
        SceneManager.LoadScene(index);
    }

    public void SetResolution()
    {
        float targetWidth = 1080f;
        float targetHeight = 1920f;
        float targetAspect = targetWidth / targetHeight;

        float windowAspect = (float)Screen.width / (float)Screen.height;

        if (windowAspect > targetAspect)
        {
            float inset = targetAspect / windowAspect;
            Camera.main.rect = new Rect((1f - inset) / 2f, 0f, inset, 1f);
        }
        else
        {
            float inset = windowAspect / targetAspect;
            Camera.main.rect = new Rect(0f, (1f - inset) / 2f, 1f, inset);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetResolution();
    }

    public int GetTokens() { return tokens; }
    public void AddTokens(int tokens) { this.tokens += tokens; }
}