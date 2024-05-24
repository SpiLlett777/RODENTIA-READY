using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int coinCount = 0;
    public TextMeshProUGUI coinCounterText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateCoinCounter();
    }

    public void SpendCoins(int amount)
    {
        coinCount -= amount;
        UpdateCoinCounter();
    }

    private void UpdateCoinCounter()
    {
        coinCounterText.text = coinCount.ToString();
    }
}