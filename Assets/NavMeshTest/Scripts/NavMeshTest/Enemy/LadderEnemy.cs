using UnityEngine;

/// <summary>
/// 梯子を制作する敵の動作を管理するクラス。
/// </summary>
public class LadderEnemy : EnemyMove
{
    [Header("梯子設定")]
    [SerializeField] private float ladderMoveSpeed = 1f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float buildTime = 5f;
    [SerializeField] private GameObject ladderPrefab;
    [SerializeField] private float ladderOffset = 0.01f;

    private LadderPointManager pointManager;
    private Vector3 ladderTarget;
    private GameObject ladderObject;
    private LadderVisual ladderVisual;
    private float buildProgress;
    private float obstacleHeight;
    private bool isBuilding;
    private bool isInitialized;

    /// <summary>
    /// 梯子敵を初期化する。
    /// </summary>
    public void Init(LadderPointManager manager, Vector3 targetPoint)
    {
        pointManager = manager;
        ladderTarget = targetPoint;

        if (agent == null || ladderPrefab == null)
        {
            Die();
            return;
        }

        agent.speed = speed;
        agent.SetDestination(ladderTarget);
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || agent == null) return;

        if (isBuilding)
        {
            BuildLadder();
            return;
        }

        agent.speed = agent.isOnOffMeshLink ? ladderMoveSpeed : speed;

        if (Vector3.Distance(transform.position, ladderTarget) <= attackRange)
        {
            StartBuilding();
        }
    }

    /// <summary>
    /// 梯子の制作を開始する。
    /// </summary>
    private void StartBuilding()
    {
        isBuilding = true;
        agent.isStopped = true;

        if (pointManager == null || !pointManager.TryGetObstacle(this, out BoxCollider obstacle))
        {
            Die();
            return;
        }

        obstacleHeight = obstacle.bounds.size.y;
        CreateLadder(obstacle);
    }

    /// <summary>
    /// 梯子を生成する。
    /// </summary>
    private void CreateLadder(BoxCollider obstacle)
    {
        Vector3 position = ladderTarget;
        position.y = obstacle.bounds.min.y;

        Vector3 localTowerPosition = obstacle.transform.InverseTransformPoint(target.transform.position);
        Vector3 center = obstacle.center;

        bool useXFace = Mathf.Abs(localTowerPosition.x - center.x) > Mathf.Abs(localTowerPosition.z - center.z);

        Vector3 direction = useXFace
            ? obstacle.transform.right * (localTowerPosition.x > center.x ? -1f : 1f)
            : obstacle.transform.forward * (localTowerPosition.z > center.z ? -1f : 1f);

        position += direction * ladderOffset;

        ladderObject = Instantiate(ladderPrefab, position, Quaternion.identity, pointManager.transform);
        ladderVisual = ladderObject.GetComponent<LadderVisual>();

        if (ladderVisual == null)
        {
            Debug.LogError("LadderPrefabにLadderVisualがありません。", ladderObject);
            Destroy(ladderObject);
            ladderObject = null;
            Die();
            return;
        }

        ladderVisual.SetHeight(0.01f);
    }

    /// <summary>
    /// 梯子の制作を進める。
    /// </summary>
    private void BuildLadder()
    {
        if (ladderVisual == null) return;

        buildProgress = Mathf.Clamp01(buildProgress + Time.deltaTime / Mathf.Max(buildTime, 0.01f));
        ladderVisual.SetHeight(obstacleHeight * buildProgress);

        if (buildProgress < 1f) return;

        ladderVisual.CompleteLadder();
        pointManager.CompletePoint(this);
        Die();
    }

    public override void Die()
    {
        if (pointManager != null) pointManager.ReleasePoint(this);
        base.Die();
    }
}
