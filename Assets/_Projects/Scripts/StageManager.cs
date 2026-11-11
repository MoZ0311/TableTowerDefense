
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

/// <summary>
/// ゲームステージの管理
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARTrackedImageManager trackedImageManager;
    [SerializeField] LineRenderer lineRenderer;

    [Header("Root")]
    [SerializeField] Transform boardRoot;
    [SerializeField] Transform towerRoot;

    [Header("Scripts")]
    [SerializeField] MarkerTracking markerTracking;
    [SerializeField] BoundaryPlaneManager boundaryPlaneManager;

    readonly List<Vector3> boundaryPoints = new();      // BoardRootを基準としたステージ境界の頂点

    void OnEnable()
    {
        // プレーン変動時のイベント追加
        if (planeManager != null)
        {
            planeManager.trackablesChanged.AddListener(OnPlaneChanged);
        }

        // 画像検出時のイベント購読
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
        }
    }

    void OnDisable()
    {
        // プレーン変動時のイベント解除
        if (planeManager != null)
        {
            planeManager.trackablesChanged.RemoveListener(OnPlaneChanged);
        }

        //イベント購読の解除
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
        }
    }

    /// <summary>
    /// Planeが更新されたときの処理
    /// </summary>
    void OnPlaneChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        UpdateBoardRootPose(boundaryPlaneManager.SelectedPlane);
        UpdateBoundaryPoints(boundaryPlaneManager.SelectedPlane);
        UpdateBoundaryLine();
    }

    /// <summary>
    /// 登録されたマーカーの状態が変化した時に呼ばれる処理
    /// </summary>
    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
    {
        UpdateTowerPosition();
    }

    /// <summary>
    /// マーカーの位置をBoardRoot基準に変換し、タワーの配置位置を更新する
    /// </summary>
    void UpdateTowerPosition()
    {
        if (boardRoot == null || towerRoot == null)
        {
            return;
        }

        // マーカーのワールド座標をBoardRootのローカル座標に変換
        Vector3 localPosition = boardRoot.InverseTransformPoint(markerTracking.MarkerTransform.position);

        // タワーの高さをBoardRootの原点と同じ高さにする
        localPosition.y = 0f;

        towerRoot.localPosition = localPosition;
    }

    /// <summary>
    /// 選択中のPlaneに合わせてBoardRootを配置する
    /// </summary>
    void UpdateBoardRootPose(ARPlane plane)
    {
        if (boardRoot == null)
        {
            Debug.LogError("boardRoot is null");
            return;
        }

        if (plane == null)
        {
            boardRoot.gameObject.SetActive(false);
            return;
        }

        boardRoot.SetPositionAndRotation(
            plane.transform.position,
            plane.transform.rotation
        );

        boardRoot.gameObject.SetActive(true);
    }

    /// <summary>
    /// 選択中のPlaneからステージ境界を取得する
    /// </summary>
    void UpdateBoundaryPoints(ARPlane plane)
    {
        boundaryPoints.Clear();

        if (plane == null || boardRoot == null)
        {
            return;
        }

        var boundary = plane.boundary;

        // ループが作れない時、return
        if (boundary.Length < 3)
        {
            return;
        }

        foreach (Vector2 point in boundary)
        {
            // Planeローカル座標をワールド座標に変換
            Vector3 worldPoint = plane.transform.TransformPoint(
                new Vector3(point.x, 0f, point.y)
            );

            // BoardRootローカル座標に変換
            Vector3 localPoint = boardRoot.InverseTransformPoint(worldPoint);

            boundaryPoints.Add(localPoint);
        }
    }

    /// <summary>
    /// ステージ境界を描画する
    /// </summary>
    void UpdateBoundaryLine()
    {
        if (lineRenderer == null)
        {
            return;
        }
        else if (boundaryPoints.Count < 3)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = boundaryPoints.Count;
        lineRenderer.SetPositions(boundaryPoints.ToArray());
        lineRenderer.enabled = true;
    }
}
