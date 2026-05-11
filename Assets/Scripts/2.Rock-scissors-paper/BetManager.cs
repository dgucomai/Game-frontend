using UnityEngine;
using UnityEngine.EventSystems;

public class BetManager : MonoBehaviour
{
    public int currentTokens, currentBet;
    public bool isBettingRequestActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTokens = GameManager.Instance.GetTokens();
        currentBet = 0;
        isBettingRequestActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBet(int amount)
    {
        if (currentTokens >= currentBet + amount)
        {
            currentBet += amount;
        }
    }

    public void ResetBet()
    {
        currentBet = 0;
    }

    public void ConfirmBetAndPlay(int userChoice)
    {
        if (isBettingRequestActive)
        {
            return;
        }
        isBettingRequestActive = true;
        currentTokens -= currentBet;
        GameManager.Instance.AddTokens(-currentBet);    
    }

    
}
