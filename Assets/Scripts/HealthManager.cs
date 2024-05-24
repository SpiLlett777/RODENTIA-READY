using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 5;
    public int maxHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [SerializeField] private GameObject person;
    private Invulnerability invulnerability;

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    public float knockbackForce = 5f;

    private GameManager gameManager;
    private void Awake()
    {
        health = maxHealth;
        invulnerability = person.GetComponent<Invulnerability>();
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].enabled = i < maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invulnerability != null && !invulnerability.isInvulnerable)
        {
            health -= damage;
            if (health <= 0)
            {
                int coinsToDrop = gameManager.coinCount;
                gameManager.SpendCoins(coinsToDrop);
                DropCoins(coinsToDrop);
            }

            invulnerability.StartColorChange();
        }
    }

    private void DropCoins(int coinsToDrop)
    {
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            Vector3 spawnPosition = person.transform.position + randomOffset;

            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            if (coinRb != null)
            {
                Vector2 forceDirection = randomOffset.normalized;
                coinRb.AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
        Debug.Log("Coins dropped: " + coinsToDrop);
    }

    public bool IsInvulnerable()
    {
        return invulnerability.isInvulnerable;
    }

    public void RestoreHealth()
    {
        health = maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }
}