using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Transform enemyRoot;
    [SerializeField] float spawnHeight = 0.1f;
    [SerializeField] int maxAttempts = 100;

    /// <summary>
    /// ステージ内に指定数の敵を生成する
    /// boundaryPointsはBoardRootローカル座標
    /// </summary>
    public void SpawnEnemies(IReadOnlyList<Vector3> boundaryPoints, int spawnCount)
    {
        if (boundaryPoints == null || boundaryPoints.Count < 3)
        {
            return;
        }

        if (!TryGetBounds(
            boundaryPoints,
            out float minX,
            out float maxX,
            out float minZ,
            out float maxZ))
            return;

        int spawnedCount = 0;

        for (int i = 0; i < spawnCount; i++)
        {
            if (!TryGetSpawnPosition(
                boundaryPoints,
                minX, maxX,
                minZ, maxZ,
                out Vector3 spawnPosition))
            {
                Debug.LogWarning("Failed to find spawn position.");
                break;
            }

            var enemy = Instantiate(enemyPrefab, enemyRoot, false);
            enemy.transform.localPosition = spawnPosition;

            spawnedCount++;
        }

        Debug.Log($"Spawned {spawnedCount} enemies.");
    }

    bool TryGetBounds(
        IReadOnlyList<Vector3> points,
        out float minX,
        out float maxX,
        out float minZ,
        out float maxZ)
    {
        minX = float.MaxValue;
        maxX = float.MinValue;
        minZ = float.MaxValue;
        maxZ = float.MinValue;

        foreach (Vector3 point in points)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minZ = Mathf.Min(minZ, point.z);
            maxZ = Mathf.Max(maxZ, point.z);
        }

        return maxX > minX && maxZ > minZ;
    }

    bool TryGetSpawnPosition(
        IReadOnlyList<Vector3> boundaryPoints,
        float minX,
        float maxX,
        float minZ,
        float maxZ,
        out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);

            Vector3 candidate = new(x, 0f, z);

            if (IsInsidePolygon(candidate, boundaryPoints))
            {
                spawnPosition = candidate + Vector3.up * spawnHeight;
                return true;
            }
        }

        return false;
    }

    bool IsInsidePolygon(
        Vector3 point,
        IReadOnlyList<Vector3> polygon)
    {
        bool inside = false;

        for (int i = 0, j = polygon.Count - 1;
             i < polygon.Count;
             j = i++)
        {
            Vector3 a = polygon[i];
            Vector3 b = polygon[j];

            bool intersects =
                (a.z > point.z) != (b.z > point.z) &&
                point.x <
                (b.x - a.x) * (point.z - a.z) /
                (b.z - a.z) + a.x;

            if (intersects)
                inside = !inside;
        }

        return inside;
    }
}
