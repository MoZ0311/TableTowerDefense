using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoundaryPlaneManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARPlaneManager planeManager;

    [Header("Settings")]
    [SerializeField] float maxPlaneDistance;

    [Header("Scripts")]
    [SerializeField] MarkerTracking markerTracking;

    public ARPlane SelectedPlane { get; private set; }  // マーカーを内包するPlane

    void OnEnable()
    {
        // プレーン変動時のイベント追加
        if (planeManager != null)
        {
            planeManager.trackablesChanged.AddListener(OnPlaneChanged);
        }
    }

    void OnDisable()
    {
        // プレーン変動時のイベント解除
        if (planeManager != null)
        {
            planeManager.trackablesChanged.RemoveListener(OnPlaneChanged);
        }
    }

    void OnPlaneChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        SelectedPlane = FindCandidatePlane();
        UpdatePlaneVisibility();
    }

    /// <summary>
    /// マーカーを内包する最適なPlaneを見つける処理
    /// </summary>
    /// <returns>マーカーを内包するARPlane</returns>
    ARPlane FindCandidatePlane()
    {
        // nullチェック
        if (planeManager == null)
        {
            Debug.LogError("planeManager is null");
            return null;
        }
        else if (markerTracking == null || markerTracking.MarkerTransform == null)
        {
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
            Vector3 localPosition = plane.transform.InverseTransformPoint(markerTracking.MarkerTransform.position);

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
                visualizer.enabled = plane == SelectedPlane;
            }
        }
    }
}
