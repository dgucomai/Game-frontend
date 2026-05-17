using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{

    public static int score = 0;
    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();

        
    }

    void Update()
    {
        text.text = score.ToString();
    }

    // 게임 오버 시 호출
    public void GiveReward()
    {
        int reward = 0;

        if (score >= 110)
        {
            reward = 50;
        }
        else if (score >= 100)
        {
            reward = 40;
        }
        else if (score >= 90)
        {
            reward = 27;
        }

        else if (score >= 80)
        {
            reward = 24;
        }


        else if (score >= 70)
        {
            reward = 21;
        }

        else if (score >= 60)
        {
            reward = 17;
        }

        else if (score >= 50)
        {
            reward = 12;
        }
        else if (score >= 40)
        {
            reward = 9;
        }
        else if (score >= 30)
        {
            reward = 7;
        }
        else if (score >= 20)
        {
            reward = 5;
        }




        if (reward > 0)
        {
            int currentTokens = GameManager.Instance.GetTokens();

            GameManager.Instance.AddTokens(reward);

            Debug.Log(reward + " 토큰 지급!");
        }
    }

    // 점수 초기화
    public void ResetScore()
    {
        score = 0;
    }
}