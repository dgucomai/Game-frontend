using UnityEngine;

public class score_up : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Score.score++;
    }
}
