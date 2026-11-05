using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoundaryPlaneManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] LineRenderer fixedBoundaryLine;

    [Header("Settings")]
    [SerializeField] float maxPlaneDistance;

    Transform markerTransform;  // 認識したマーカーのTransform情報
    ARPlane selectedPlane;      // 描画対象である、マーカーを内包するPlane

    void Start()
    {
        fixedBoundaryLine.enabled = false;
    }

    /// <summary>
    /// マーカーのTransformを設定する処理
    /// </summary>
    /// <param name="targetTransform">マーカーのTransform</param>
    public void SetMarkerTransform(Transform targetTransform)
    {
        markerTransform = targetTransform;
        UpdateSelectedPlane();
    }

    /// <summary>
    /// 最適な平面(マーカーに近い平面)の更新処理
    /// </summary>
    void UpdateSelectedPlane()
    {
        selectedPlane = FindCandidatePlane();
        UpdatePlaneVisibility();
    }

    /// <summary>
    /// マーカーを内包する最適なPlaneを見つける処理
    /// </summary>
    /// <returns>マーカーを内包するARPlane</returns>
    ARPlane FindCandidatePlane()
    {
        // nullチェック
        if (planeManager == null || markerTransform == null)
        {
            Debug.LogError("planeManager is null");
            return null;
        }

        ARPlane bestPlane = null;
        float bestDistance = float.MaxValue;

        foreach (var plane in planeManager.trackables)
        {
            // 平面が水平上向きでなければ、continue
            if (plane.alignment != PlaneAlignment.HorizontalUp)
            {
                continue;
            }

            // ARPlaneの座標を、マーカー座標を基準に計算
            Vector3 localPosition = plane.transform.InverseTransformPoint(markerTransform.position);

            // 平面の高さの差(y方向の距離)を絶対値で取得
            float distance = Mathf.Abs(localPosition.y);

            // 距離が閾値を超えていればcontinue
            if (distance > maxPlaneDistance)
            {
                continue;
            }

            // マーカーを内包するかチェックし、不適であればcontinue
            Vector2 planePoint = new(localPosition.x, localPosition.z);
            if (!IsInsideBoundary(plane, planePoint))
            {
                continue;
            }

            // より近いPlaneがあれば、更新
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPlane = plane;
            }
        }

        return bestPlane;
    }

    /// <summary>
    /// 指定位置がPlaneの境界内にあるか判定する処理
    /// </summary>
    /// <param name="plane">検査対象のPlane</param>
    /// <param name="point"></param>
    /// <returns>指定位置がPlaneの境界内にあるか</returns>
    bool IsInsideBoundary(ARPlane plane, Vector2 point)
    {
        // 境界の頂点が3つ未満であれば、面を構成出来ない
        var boundary = plane.boundary;
        if (boundary.Length < 3)
        {
            return false;
        }

        bool isInside = false;

        for (int i = 0, j = boundary.Length - 1;
             i < boundary.Length;
             j = i++)
        {
            Vector2 a = boundary[i];
            Vector2 b = boundary[j];

            bool crosses =
                (a.y > point.y) != (b.y > point.y) &&
                point.x <
                (b.x - a.x) * (point.y - a.y) /
                (b.y - a.y) + a.x;

            if (crosses)
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }

    /// <summary>
    /// 選択中のPlaneだけ表示する処理
    /// </summary>
    void UpdatePlaneVisibility()
    {
        // nullチェック
        if (planeManager == null)
        {
            Debug.LogError("planeManager is null");
            return;
        }

        foreach (ARPlane plane in planeManager.trackables)
        {
            // 表示用コンポーネントを、selectedPlaneとの比較で設定
            if (plane.TryGetComponent<ARPlaneMeshVisualizer>(out var visualizer))
            {
                visualizer.enabled = plane == selectedPlane;
            }
        }
    }
}
