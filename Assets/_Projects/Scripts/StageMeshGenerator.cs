
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ステージ境界から床メッシュを生成するクラス
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StageMeshGenerator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;

    [Header("Settings")]
    [SerializeField] float surfaceOffset = 0.005f;

    const float Epsilon = 0.000001f;

    Mesh generatedMesh;

    /// <summary>
    /// 境界情報から床メッシュを生成する
    /// </summary>
    public bool Generate(IReadOnlyList<Vector3> boundaryPoints)
    {
        if (boundaryPoints == null || boundaryPoints.Count < 3)
        {
            Debug.LogWarning("Invalid boundary points.");
            return false;
        }

        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        // 境界頂点を整理
        List<Vector3> vertices = CleanPolygon(boundaryPoints);

        if (vertices.Count < 3)
            return false;

        // 三角形分割
        if (!TryTriangulate(vertices, out List<int> triangles))
        {
            Debug.LogWarning("Failed to triangulate stage boundary.");
            return false;
        }

        // 床面の高さを少し上げる
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] += Vector3.up * surfaceOffset;
        }

        // Mesh生成
        Mesh mesh = new Mesh();
        mesh.name = "StageMesh";

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        // 法線を上向きに設定
        Vector3[] normals = new Vector3[vertices.Count];

        for (int i = 0; i < normals.Length; i++)
            normals[i] = Vector3.up;

        mesh.normals = normals;

        // UV座標を生成
        mesh.uv = GenerateUV(vertices);

        mesh.RecalculateBounds();

        // 古いMeshを破棄
        if (generatedMesh != null)
            Destroy(generatedMesh);

        generatedMesh = mesh;
        meshFilter.sharedMesh = generatedMesh;

        // Colliderが設定されていれば更新
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = generatedMesh;
        }

        return true;
    }

    /// <summary>
    /// 重複頂点や一直線上の頂点を除去
    /// </summary>
    List<Vector3> CleanPolygon(IReadOnlyList<Vector3> points)
    {
        List<Vector3> result = new();

        // 隣接する重複頂点を除去
        foreach (Vector3 point in points)
        {
            if (result.Count == 0 ||
                (result[^1] - point).sqrMagnitude > Epsilon * Epsilon)
            {
                result.Add(point);
            }
        }

        // 始点と終点が重複していれば除去
        if (result.Count > 1 &&
            (result[0] - result[^1]).sqrMagnitude <= Epsilon * Epsilon)
        {
            result.RemoveAt(result.Count - 1);
        }

        // 一直線上の頂点を除去
        bool removed = true;

        while (removed && result.Count >= 3)
        {
            removed = false;

            for (int i = 0; i < result.Count; i++)
            {
                Vector3 a = result[(i - 1 + result.Count) % result.Count];
                Vector3 b = result[i];
                Vector3 c = result[(i + 1) % result.Count];

                if (Mathf.Abs(Cross2D(a, b, c)) <= Epsilon)
                {
                    result.RemoveAt(i);
                    removed = true;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Ear Clippingによる三角形分割
    /// </summary>
    bool TryTriangulate(
        List<Vector3> vertices,
        out List<int> triangles)
    {
        triangles = new();

        int count = vertices.Count;

        // ポリゴンの回転方向を取得
        float area = CalculateArea(vertices);

        if (Mathf.Abs(area) <= Epsilon)
            return false;

        float orientation = Mathf.Sign(area);

        // 未処理の頂点インデックス
        List<int> remaining = new();

        for (int i = 0; i < count; i++)
            remaining.Add(i);

        // 耳を順番に切り出す
        while (remaining.Count > 3)
        {
            bool foundEar = false;

            for (int i = 0; i < remaining.Count; i++)
            {
                int prevIndex =
                    remaining[(i - 1 + remaining.Count) % remaining.Count];

                int currentIndex = remaining[i];

                int nextIndex =
                    remaining[(i + 1) % remaining.Count];

                Vector3 a = vertices[prevIndex];
                Vector3 b = vertices[currentIndex];
                Vector3 c = vertices[nextIndex];

                // 凸頂点でなければ耳ではない
                if (Cross2D(a, b, c) * orientation <= Epsilon)
                    continue;

                // 他の頂点が三角形内部に存在するか
                bool containsPoint = false;

                foreach (int index in remaining)
                {
                    if (index == prevIndex ||
                        index == currentIndex ||
                        index == nextIndex)
                    {
                        continue;
                    }

                    if (IsInsideTriangle(
                        vertices[index], a, b, c, orientation))
                    {
                        containsPoint = true;
                        break;
                    }
                }

                if (containsPoint)
                    continue;

                // 三角形を追加
                AddTriangle(
                    triangles,
                    prevIndex,
                    currentIndex,
                    nextIndex,
                    orientation
                );

                remaining.RemoveAt(i);

                foundEar = true;
                break;
            }

            // 耳が見つからない場合は分割失敗
            if (!foundEar)
                return false;
        }

        // 最後に残った三角形
        AddTriangle(
            triangles,
            remaining[0],
            remaining[1],
            remaining[2],
            orientation
        );

        return true;
    }

    /// <summary>
    /// 三角形を上向きの法線になる順序で追加
    /// </summary>
    void AddTriangle(
        List<int> triangles,
        int a,
        int b,
        int c,
        float orientation)
    {
        // XZ平面の反時計回りは、Unityの3D空間では下向きになる
        if (orientation > 0f)
        {
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
        }
        else
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
    }

    /// <summary>
    /// 三角形内部に点が存在するか判定
    /// </summary>
    bool IsInsideTriangle(
        Vector3 point,
        Vector3 a,
        Vector3 b,
        Vector3 c,
        float orientation)
    {
        float cross1 = Cross2D(a, b, point) * orientation;
        float cross2 = Cross2D(b, c, point) * orientation;
        float cross3 = Cross2D(c, a, point) * orientation;

        return cross1 >= -Epsilon &&
               cross2 >= -Epsilon &&
               cross3 >= -Epsilon;
    }

    /// <summary>
    /// XZ平面上の外積
    /// </summary>
    float Cross2D(Vector3 a, Vector3 b, Vector3 c)
    {
        return (b.x - a.x) * (c.z - a.z) -
               (b.z - a.z) * (c.x - a.x);
    }

    /// <summary>
    /// ポリゴンの符号付き面積
    /// </summary>
    float CalculateArea(List<Vector3> vertices)
    {
        float area = 0f;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 a = vertices[i];
            Vector3 b = vertices[(i + 1) % vertices.Count];

            area += a.x * b.z - b.x * a.z;
        }

        return area * 0.5f;
    }

    /// <summary>
    /// 床用UV座標を生成
    /// </summary>
    Vector2[] GenerateUV(List<Vector3> vertices)
    {
        Vector2[] uv = new Vector2[vertices.Count];

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (Vector3 vertex in vertices)
        {
            minX = Mathf.Min(minX, vertex.x);
            maxX = Mathf.Max(maxX, vertex.x);
            minZ = Mathf.Min(minZ, vertex.z);
            maxZ = Mathf.Max(maxZ, vertex.z);
        }

        float width = Mathf.Max(maxX - minX, Epsilon);
        float depth = Mathf.Max(maxZ - minZ, Epsilon);

        for (int i = 0; i < vertices.Count; i++)
        {
            uv[i] = new Vector2(
                (vertices[i].x - minX) / width,
                (vertices[i].z - minZ) / depth
            );
        }

        return uv;
    }

    void OnDestroy()
    {
        if (generatedMesh != null)
        {
            Destroy(generatedMesh);
        }
    }
}
