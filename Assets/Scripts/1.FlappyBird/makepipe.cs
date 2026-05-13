using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class makepipe : MonoBehaviour
{
    public GameObject pipe;

    float timer = 0;

    public float timeDiff = 2f;

    // 기본 속도
    public float pipeSpeed = 2f;

    void Update()
    {
        // 점수에 따라 난이도 증가
        if (Score.score >= 80)
        {
            timeDiff = 1.4f;
        }
        else if (Score.score >= 50)
        {
            timeDiff = 1.6f;
        }
        else if (Score.score >= 30)
        {
            timeDiff = 1.8f;
            
        }
        else if (Score.score >= 10)
        {
            timeDiff = 1.9f;
            
        }
        else
        {
            timeDiff = 2f;
        }

        timer += Time.deltaTime;

        if (timer > timeDiff)
        {
            GameObject newpipe = Instantiate(pipe);

            newpipe.transform.position =
                new Vector3(3, Random.Range(-2.5f, 2f), 0);

            // 생성된 파이프의 속도 설정
            newpipe.GetComponent<pipe_move>().speed = pipeSpeed;

            timer = 0;

            Destroy(newpipe, 7.0f);
        }
    }
}