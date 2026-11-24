using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected enum EnemyState
    {
        None,
        Move,
        Attack,
        Skill,
        Die
    }

    [SerializeField] float moveSpeed;
    [SerializeField] float maxHealth;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackInterval;

    protected Tower tower;
    protected Transform target;
    protected EnemyState enemyState;
    float currentHealth;
    float attackTimer;

    void OnEnable()
    {
        // Towerスクリプトの検索、取得
        tower = FindAnyObjectByType<Tower>();
        if (tower == null)
        {
            Debug.LogError("Tower Not Found");
            //Destroy(gameObject);
        }

        tower.OnTowerDestroyed += () => enemyState = EnemyState.None;

        // ステータスの初期化
        InitializeStatus();
    }

    void OnDisable()
    {
        if (tower != null)
        {
            tower.OnTowerDestroyed -= () => enemyState = EnemyState.None;
        }
    }

    void InitializeStatus()
    {
        target = tower.transform;
        enemyState = EnemyState.Move;
        currentHealth = maxHealth;
        attackTimer = attackInterval;
    }

    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Move:
                UpdateMove();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            
            default:
                return;
        }
        UpdateState();
    }

    public virtual void TakeDamage(float damageAmount)
    {
        // 0以下にならないように調整
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        if (currentHealth <= 0)
        {
            enemyState = EnemyState.Die;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    void UpdateState()
    {
        if (enemyState == EnemyState.None)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            enemyState = EnemyState.Attack;
        }
        else
        {
            enemyState = EnemyState.Move;
        }
    }

    protected virtual void UpdateMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
    }

    protected virtual void UpdateAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            tower.TakeDamage(attackDamage);
            attackTimer = attackInterval;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
