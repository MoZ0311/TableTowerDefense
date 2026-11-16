using UnityEngine;

/// <summary>
/// 近距離攻撃を行う敵の動作を管理するクラス。
/// </summary>
public class MeleeEnemy : EnemyMove
{
    [Header("近距離攻撃")]
    [SerializeField] private float attackRange = 1f;

    private void Start()
    {
        if (target != null) MoveToTower();
    }

    private void Update()
    {
        if (target == null || agent == null) return;

        float distance = GetTargetDistance();

        if (distance <= attackRange && CanAttackTarget())
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        if (!agent.pathPending && !agent.hasPath) MoveToTower();
    }
}
