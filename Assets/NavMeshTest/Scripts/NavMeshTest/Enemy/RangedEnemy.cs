using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 遠距離攻撃を行う敵の動作を管理するクラス。
/// </summary>
public class RangedEnemy : EnemyMove
{
    [Header("攻撃設定")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackInterval = 1f;

    private RangedAttackPointManager pointManager;
    private Vector3 attackPoint;
    private float nextAttackTime;
    private bool hasAttackPoint;
    private bool arrivedAtAttackPoint;

    private void Start()
    {
        pointManager = FindAnyObjectByType<RangedAttackPointManager>();

        if (target == null || agent == null) return;

        if (pointManager != null && pointManager.TryReserveAttackPoint(transform.position, this, out attackPoint))
        {
            hasAttackPoint = true;
            agent.SetDestination(attackPoint);
        }
        else
        {
            MoveToTower();
        }
    }

    private void Update()
    {
        if (target == null || agent == null || !agent.isOnNavMesh) return;

        if (hasAttackPoint)
        {
            UpdateAttackPoint();
            return;
        }

        float distance = GetTargetDistance();

        if (distance <= attackRange && CanAttackTarget())
        {
            agent.isStopped = true;
            AttackTower();
            return;
        }

        agent.isStopped = false;

        if (!agent.pathPending && !agent.hasPath) MoveToTower();
    }

    /// <summary>
    /// 狙撃ポイントでの行動を管理する。
    /// </summary>
    private void UpdateAttackPoint()
    {
        if (!arrivedAtAttackPoint)
        {
            if (agent.pathPending) return;

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                AbandonAttackPoint();
                return;
            }

            if (!agent.hasPath || agent.remainingDistance > agent.stoppingDistance + 0.05f) return;

            arrivedAtAttackPoint = true;
            agent.isStopped = true;
            agent.ResetPath();
        }

        agent.isStopped = true;
        LookAtTarget();
        AttackTower();
    }

    private void AbandonAttackPoint()
    {
        pointManager?.ReleaseAttackPoint(this);
        hasAttackPoint = false;
        arrivedAtAttackPoint = false;
        agent.isStopped = false;
        MoveToTower();
    }

    /// <summary>
    /// タワーを攻撃する。
    /// </summary>
    private void AttackTower()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackInterval;

        // TODO: タワーのHPを減らす処理
    }

    private void LookAtTarget()
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, agent.angularSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (pointManager != null) pointManager.ReleaseAttackPoint(this);
    }
}
