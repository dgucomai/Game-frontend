using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
public class Score : MonoBehaviour
{
    public  static int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = score.ToString();
    }
}
