using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 全ての敵に共通する動作を管理する親クラス。
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyMove : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] protected float speed = 1f;

    [Header("タワーのレイヤー")]
    [SerializeField] protected LayerMask towerLayer;

    protected NavMeshAgent agent;
    protected GameObject target;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        target = GameObject.FindGameObjectWithTag("Tower");
    }

    /// <summary>
    /// タワーの方向と距離を取得する。
    /// </summary>
    protected bool TryGetTowerDirection(out Vector3 direction, out float distance)
    {
        direction = Vector3.zero;
        distance = 0f;

        if (target == null) return false;

        direction = target.transform.position - transform.position;
        distance = direction.magnitude;

        return true;
    }

    /// <summary>
    /// タワーとの距離を取得する。
    /// </summary>
    protected float GetTargetDistance()
    {
        return target == null ? float.MaxValue : Vector3.Distance(transform.position, target.transform.position);
    }

    /// <summary>
    /// タワーに向かって移動する。
    /// </summary>
    protected void MoveToTower()
    {
        if (!TryGetTowerDirection(out Vector3 direction, out float distance)) return;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, towerLayer))
        {
            agent.SetDestination(hit.point);
        }
    }

    /// <summary>
    /// タワーとの間に障害物がないか判定する。
    /// </summary>
    protected bool CanAttackTarget()
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 destination = target.transform.position + Vector3.up * 0.5f;
        Vector3 direction = destination - origin;

        if (direction.sqrMagnitude <= 0.0001f) return true;

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            return hit.transform == target.transform || hit.transform.IsChildOf(target.transform);
        }

        return false;
    }

    [ContextMenu("死亡")]
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
