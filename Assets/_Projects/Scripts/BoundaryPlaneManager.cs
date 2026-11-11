using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class BoundaryPlaneManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] LineRenderer boundaryLine;

    [Header("Root")]
    [SerializeField] Transform boardRoot;
    [SerializeField] Transform towerRoot;

    [Header("Settings")]
    [SerializeField] float maxPlaneDistance;

    Transform markerTransform;  // 認識したマーカーのTransform情報
    ARPlane selectedPlane;      // 描画対象である、マーカーを内包するPlane

    readonly List<Vector3> boundaryPoints = new();   // BoardRootを基準としたステージ境界の頂点

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
        UpdateSelectedPlane();
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
        UpdateBoardRootPose();
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
        else if (markerTransform == null)
        {
            Debug.LogWarning("markerTransform is null");
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

    /// <summary>
    /// 選択中のPlaneに合わせてBoardRootの位置と回転を更新する
    /// </summary>
    void UpdateBoardRootPose()
    {
        if (boardRoot == null)
        {
            Debug.LogError("boardRoot is null");
            return;
        }

        // Planeが選択されていない場合は非表示
        if (selectedPlane == null)
        {
            boardRoot.gameObject.SetActive(false);
            boundaryPoints.Clear();
            return;
        }

        // 選択中のPlaneの位置と回転を取得
        Transform planeTransform = selectedPlane.transform;

        boardRoot.SetPositionAndRotation(
            planeTransform.position,
            planeTransform.rotation
        );

        // BoardRootを有効化
        boardRoot.gameObject.SetActive(true);

        UpdateTowerPosition();

        UpdateBoundaryPoints();
    }

    /// <summary>
    /// マーカーの位置をBoardRoot基準に変換し、タワーの配置位置を更新する
    /// </summary>
    void UpdateTowerPosition()
    {
        if (boardRoot == null || towerRoot == null || markerTransform == null)
        {
            return;
        }

        // マーカーのワールド座標をBoardRootのローカル座標に変換
        Vector3 localPosition = boardRoot.InverseTransformPoint(markerTransform.position);

        // タワーの高さをBoardRootの原点と同じ高さにする
        localPosition.y = 0f;

        towerRoot.localPosition = localPosition;
    }

    /// <summary>
    /// 選択中のPlaneの境界を取得し、BoardRoot基準の座標で保存する
    /// </summary>
    void UpdateBoundaryPoints()
    {
        // 前回の境界情報をクリア
        boundaryPoints.Clear();

        if (selectedPlane == null || boardRoot == null)
        {
            return;
        }

        // Planeの境界頂点を取得
        var boundary = selectedPlane.boundary;

        // 多角形を構成できない場合は終了
        if (boundary.Length < 3)
        {
            return;
        }

        foreach (Vector2 point in boundary)
        {
            // Planeローカル座標からワールド座標に変換
            Vector3 worldPoint = selectedPlane.transform.TransformPoint(new Vector3(point.x, 0f, point.y));

            // ワールド座標からBoardRootローカル座標に変換
            Vector3 localPoint = boardRoot.InverseTransformPoint(worldPoint);

            boundaryPoints.Add(localPoint);
        }

        DrawBoundaryLine();
    }

    public void DrawBoundaryLine()
    {
        if (boundaryLine == null)
        {
            return;
        }

        if (boundaryPoints.Count < 3)
        {
            boundaryLine.positionCount = 0;
            return;
        }

        // BoardRoot基準の座標をワールド座標に変換
        Vector3[] worldPoints = new Vector3[boundaryPoints.Count];

        for (int i = 0; i < boundaryPoints.Count; i++)
        {
            worldPoints[i] = boardRoot.TransformPoint(boundaryPoints[i]);
        }

        boundaryLine.useWorldSpace = true;
        boundaryLine.loop = true;
        boundaryLine.positionCount = worldPoints.Length;
        boundaryLine.SetPositions(worldPoints);

        boundaryLine.enabled = true;
    }
}
