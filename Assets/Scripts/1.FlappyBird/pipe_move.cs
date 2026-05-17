using UnityEngine;

public class pipe_move : MonoBehaviour
{
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!akoJump.isGameStarted)
            return;
            
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
