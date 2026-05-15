using UnityEngine;
using UnityEngine.UI;

public class bc2 : MonoBehaviour
{
    public Sprite s1Background;
    public Sprite s2Background;
    public Sprite s3Background;
    public Sprite s4Background;
    public Sprite s5Background;

    Image backgroundImage;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
    }

    void Update()
    {
        if (Score.score >= 80)
        {
            backgroundImage.sprite = s5Background;
        }
        else if (Score.score >= 50)
        {
            backgroundImage.sprite = s4Background;
        }
        else if (Score.score >= 30)
        {
            backgroundImage.sprite = s3Background;
        }
        else if (Score.score >= 10)
        {
            backgroundImage.sprite = s2Background;
        }
        else
        {
            backgroundImage.sprite = s1Background;
        }
    }
}