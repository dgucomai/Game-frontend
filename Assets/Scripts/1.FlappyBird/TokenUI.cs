using UnityEngine;
using TMPro;

public class TokenUI : MonoBehaviour
{
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        text.text =
            "Token : " +
            GameManager.Instance.GetTokens().ToString();
    }
}