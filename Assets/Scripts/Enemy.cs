using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;
    int currentHealth;

    public float knockbackForce = 2f;
    public float knockbackUpForce = 4.5f;

    public Loot loot;

    void Start()
    {
        currentHealth = maxHealth;
        loot = GetComponent<Loot>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Knockback();
        }
    }

    void Die()
    {
        loot.DropCoins(transform.position);
        Destroy(gameObject);
    }

    void Knockback()
    {
        Rigidbody2D enemyRb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (enemyRb != null)
        {
            Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            enemyRb.AddForce(Vector2.up * knockbackUpForce, ForceMode2D.Impulse);
        }
    }
}