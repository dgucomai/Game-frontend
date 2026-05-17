using UnityEngine;
using UnityEngine.SceneManagement;
public class akoJump : MonoBehaviour
{
    public GameManager gm;


    public float speedPower = 3;
     public static bool isGameStarted = false;


    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isGameStarted)
            {
                isGameStarted = true;

                rb.simulated = true;
            }
            
            rb.linearVelocity = Vector2.up * speedPower;
        
            

        }
    }
	
	private void OnCollisionEnter2D(Collision2D other) {
		SceneManager.LoadScene("GameOver");
        FindFirstObjectByType<Score>().GiveReward();
	}
}