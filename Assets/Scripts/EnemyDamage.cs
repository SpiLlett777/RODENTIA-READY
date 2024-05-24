using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private HealthManager healthManager;
    [SerializeField] private int damage;
    private void Awake()
    {
        healthManager = FindFirstObjectByType<HealthManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!healthManager.IsInvulnerable())
            {
                healthManager.TakeDamage(damage);
            }
        }
    }
}