using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Point Range")]
    public Transform attackPoint;
    public float attackRange = 0.5f;

    [Space(10)]
    public LayerMask enemyLayers;

    [Space(10)]
    public int attackDamage = 2;

    [Space(10)]
    public float attackRate = 2f;
    private float attackCooldown = 0f;

    [Header("Animation")]
    [Space(10)]
    public Animator animator;

    private int attackCount = 0;
    private float comboTimer = 0f;

    [Space(10)]
    [SerializeField] private float comboMaxDelay = 1f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f && attackCount == 1)
            {
                ResetAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && attackCooldown <= 0f && !animator.GetBool("IsJumping"))
        {
            Attack();
            attackCooldown = 1f / attackRate;
            comboTimer = comboMaxDelay;
        }
    }

    void Attack()
    {
        if (comboTimer > 0f && attackCount == 1)
        {
            attackCount = 2;
        }
        else
        {
            attackCount = 1;
        }

        animator.SetInteger("AttackCount", attackCount);
        animator.SetTrigger("AttackTrigger");

        StartCoroutine(DelayedAttack(0.4f));

        if (attackCount == 2)
        {
            comboTimer = 0f;
        }
    }

    private IEnumerator DelayedAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage / 2);
        }
    }

    public void ResetAttack()
    {
        attackCount = 0;
        animator.SetInteger("AttackCount", attackCount);
    }

    public void SetAttackPointPosition(Vector2 newPosition)
    {
        attackPoint.localPosition = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}