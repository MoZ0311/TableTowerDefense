using UnityEngine;

/// <summary>
/// 敵の生成を管理するクラス。
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [Header("敵Prefab")]
    [SerializeField] private GameObject ladderEnemyPrefab;
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("梯子ポイント管理")]
    [SerializeField] private LadderPointManager ladderPointManager;

    [ContextMenu("梯子敵を生成")]
    private void SpawnLadderEnemy()
    {
        if (ladderPointManager == null || ladderEnemyPrefab == null) return;

        GameObject obj = Instantiate(ladderEnemyPrefab, transform.position, Quaternion.identity, transform);
        LadderEnemy enemy = obj.GetComponent<LadderEnemy>();

        if (enemy == null)
        {
            Debug.LogError("梯子敵PrefabにLadderEnemyがありません。", obj);
            Destroy(obj);
            return;
        }

        if (!ladderPointManager.TryReservePoint(transform.position, enemy, out Vector3 targetPoint))
        {
            Destroy(obj);
            return;
        }

        enemy.Init(ladderPointManager, targetPoint);
    }

    [ContextMenu("近距離敵を生成")]
    private void SpawnMeleeEnemy() => SpawnEnemy<MeleeEnemy>(meleeEnemyPrefab);

    [ContextMenu("遠距離敵を生成")]
    private void SpawnRangedEnemy() => SpawnEnemy<RangedEnemy>(rangedEnemyPrefab);

    /// <summary>
    /// 指定されたPrefabから敵を生成する。
    /// </summary>
    private void SpawnEnemy<T>(GameObject prefab) where T : Component
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, transform);

        if (obj.GetComponent<T>() == null)
        {
            Debug.LogError($"敵Prefabに{typeof(T).Name}がありません。", obj);
            Destroy(obj);
        }
    }
}
