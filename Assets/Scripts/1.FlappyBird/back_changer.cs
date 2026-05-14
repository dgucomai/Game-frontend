using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    public Sprite s1Background;
    public Sprite s2Background;
    public Sprite s3Background;
    public Sprite s4Background;
    public Sprite s5Background;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Score.score >= 80)
        {
            sr.sprite = s5Background;
        }
        else if (Score.score >= 50)
        {
            sr.sprite = s4Background;
        }
        else if (Score.score >= 30)
        {
            sr.sprite = s3Background;
        }
        else if (Score.score >= 10)
        {
            sr.sprite = s2Background;
        }
        else
        {
            sr.sprite = s1Background;
        }
    }
}