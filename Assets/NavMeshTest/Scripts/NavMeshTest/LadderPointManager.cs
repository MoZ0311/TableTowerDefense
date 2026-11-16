using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 梯子を設置するポイントを管理するクラス。
/// </summary>
public class LadderPointManager : MonoBehaviour
{
    [Header("タワー")]
    [SerializeField] private GameObject tower;

    [Header("障害物")]
    [SerializeField] private List<BoxCollider> obstacles = new List<BoxCollider>();

    [Header("ポイント設定")]
    [SerializeField] private float pointSpacing = 1.5f;

    private readonly List<LadderPoint> points = new List<LadderPoint>();

    private class LadderPoint
    {
        public Vector3 Position;
        public BoxCollider Obstacle;
        public LadderEnemy UsingEnemy;
        public bool IsCompleted;

        public LadderPoint(Vector3 position, BoxCollider obstacle)
        {
            Position = position;
            Obstacle = obstacle;
        }
    }

    private void Start()
    {
        CalculatePoints();
    }

    /// <summary>
    /// 全ての障害物からポイントを計算する。
    /// </summary>
    [ContextMenu("梯子ポイントを計算")]
    private void CalculatePoints()
    {
        points.Clear();

        if (tower == null)
        {
            Debug.LogError("LadderPointManager: Towerが未設定です。", this);
            return;
        }

        foreach (BoxCollider obstacle in obstacles)
        {
            if (obstacle == null) continue;

            List<Vector3> positions = CalculateObstaclePoints(obstacle);

            foreach (Vector3 position in positions) points.Add(new LadderPoint(position, obstacle));
        }
    }

    private List<Vector3> CalculateObstaclePoints(BoxCollider obstacle)
    {
        Transform t = obstacle.transform;
        Vector3 center = obstacle.center;
        Vector3 size = obstacle.size;
        Vector3 towerPosition = t.InverseTransformPoint(tower.transform.position);

        bool useXFace = Mathf.Abs(towerPosition.x - center.x) > Mathf.Abs(towerPosition.z - center.z);

        float width = useXFace ? t.TransformVector(Vector3.forward * size.z).magnitude : t.TransformVector(Vector3.right * size.x).magnitude;
        int count = Mathf.Max(1, Mathf.FloorToInt(width / Mathf.Max(pointSpacing, 0.01f)) + 1);

        return CreatePoints(obstacle, count, useXFace, towerPosition);
    }

    private List<Vector3> CreatePoints(BoxCollider obstacle, int count, bool useXFace, Vector3 towerPosition)
    {
        Transform t = obstacle.transform;
        Vector3 center = obstacle.center;
        Vector3 size = obstacle.size;
        List<Vector3> result = new List<Vector3>();

        const float widthRatio = 0.8f;

        if (useXFace)
        {
            float x = towerPosition.x > center.x ? center.x - size.x * 0.5f : center.x + size.x * 0.5f;
            float min = center.z - size.z * 0.5f * widthRatio;
            float max = center.z + size.z * 0.5f * widthRatio;

            for (int i = 0; i < count; i++)
            {
                float z = Mathf.Lerp(min, max, count == 1 ? 0.5f : (float)i / (count - 1));
                result.Add(t.TransformPoint(new Vector3(x, center.y, z)));
            }
        }
        else
        {
            float z = towerPosition.z > center.z ? center.z - size.z * 0.5f : center.z + size.z * 0.5f;
            float min = center.x - size.x * 0.5f * widthRatio;
            float max = center.x + size.x * 0.5f * widthRatio;

            for (int i = 0; i < count; i++)
            {
                float x = Mathf.Lerp(min, max, count == 1 ? 0.5f : (float)i / (count - 1));
                result.Add(t.TransformPoint(new Vector3(x, center.y, z)));
            }
        }

        return result;
    }

    /// <summary>
    /// 最も近い空きポイントを予約する。
    /// </summary>
    public bool TryReservePoint(Vector3 spawnPosition, LadderEnemy enemy, out Vector3 targetPoint)
    {
        targetPoint = Vector3.zero;

        if (enemy == null) return false;

        LadderPoint nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (LadderPoint point in points)
        {
            if (point.Obstacle == null || point.IsCompleted || point.UsingEnemy != null) continue;

            float distance = Vector3.Distance(spawnPosition, point.Position);

            if (distance >= nearestDistance) continue;

            nearestDistance = distance;
            nearest = point;
        }

        if (nearest == null) return false;

        nearest.UsingEnemy = enemy;
        targetPoint = nearest.Position;
        return true;
    }

    /// <summary>
    /// 敵が担当する障害物を取得する。
    /// </summary>
    public bool TryGetObstacle(LadderEnemy enemy, out BoxCollider obstacle)
    {
        obstacle = null;

        foreach (LadderPoint point in points)
        {
            if (point.UsingEnemy != enemy) continue;

            obstacle = point.Obstacle;
            return obstacle != null;
        }

        return false;
    }

    /// <summary>
    /// 梯子が完成したポイントを登録する。
    /// </summary>
    public void CompletePoint(LadderEnemy enemy)
    {
        foreach (LadderPoint point in points)
        {
            if (point.UsingEnemy != enemy) continue;

            point.IsCompleted = true;
            point.UsingEnemy = null;
            return;
        }
    }

    /// <summary>
    /// 使用中のポイントを解放する。
    /// </summary>
    public void ReleasePoint(LadderEnemy enemy)
    {
        foreach (LadderPoint point in points)
        {
            if (point.UsingEnemy != enemy) continue;

            point.UsingEnemy = null;
            return;
        }
    }
}
