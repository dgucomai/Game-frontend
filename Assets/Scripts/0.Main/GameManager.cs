using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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
        string url = $"api URL";

        Debug.Log("서버에 토큰 조회 요청 중...");

        /* [API 완성되면 주석 해제]
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 예: {"tokens": 5000} 형태의 응답을 파싱
                // this.tokens = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text).tokens;
                Debug.Log("토큰 로드 성공!");
            }
        }
        */

        // --- 임시 데이터 ---
        yield return new WaitForSeconds(1.0f);
        this.tokens = 10000;
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