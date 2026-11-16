
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

/// <summary>
/// ゲームステージの管理
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] LineRenderer lineRenderer;

    [Header("Root")]
    [SerializeField] Transform boardRoot;
    [SerializeField] Transform towerRoot;

    [Header("Prefabs")]
    [SerializeField] GameObject towerInGamePrefab;

    [Header("Scripts")]
    [SerializeField] BoundaryPlaneManager boundaryPlaneManager;
    [SerializeField] MarkerTracking markerTracking;
    [SerializeField] StageMeshGenerator stageMeshGenerator;

    public bool CanFinalizeStage
    {
        get =>
        !HasFinalizedStage &&
        boardRoot.gameObject.activeInHierarchy &&
        boundaryPoints.Count >= 3;
    }
    public bool HasFinalizedStage { get; private set; }

    readonly List<Vector3> boundaryPoints = new();          // BoardRootを基準としたステージ境界の頂点
    readonly List<Vector3> finalizedBoundaryPoints = new(); // 確定したステージ境界の頂点情報
    Vector3 finalizedBoardPosition;
    Quaternion finalizedBoardRotation;
    Vector3 finalizedTowerPosition;

    void OnEnable()
    {
        // プレーン変動時のイベント追加
        if (boundaryPlaneManager != null)
        {
            boundaryPlaneManager.PlaneUpdate += OnPlaneChanged;
        }

        // 画像検出時のイベント購読
        if (markerTracking != null)
        {
            markerTracking.MarkerUpdate += OnImageChanged;
        }
    }

    void OnDisable()
    {
        // プレーン変動時のイベント解除
        if (boundaryPlaneManager != null)
        {
            boundaryPlaneManager.PlaneUpdate -= OnPlaneChanged;
        }

        //イベント購読の解除
        if (markerTracking != null)
        {
            markerTracking.MarkerUpdate -= OnImageChanged;
        }
    }

    /// <summary>
    /// Planeが更新されたときの処理
    /// </summary>
    void OnPlaneChanged(ARPlane plane)
    {
        if (plane == null ||
            plane.trackingState != TrackingState.Tracking)
        {
            boundaryPoints.Clear();
            if (boardRoot != null)
            {
                boardRoot.gameObject.SetActive(false);
            }
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
            }
            return;
        }

        UpdateBoardRootPose(plane);
        UpdateBoundaryPoints(plane);
        UpdateBoundaryLine();

        UpdateTowerPosition();
    }

    /// <summary>
    /// 登録されたマーカーの状態が変化した時に呼ばれる処理
    /// </summary>
    void OnImageChanged(ARTrackedImage image)
    {
        if (image == null ||
            image.trackingState != TrackingState.Tracking)
        {
            return;
        }

        UpdateTowerPosition();
    }

    /// <summary>
    /// タワー(メインゲームで使う本物)の座標を設定する処理
    /// </summary>
    void UpdateTowerPosition()
    {
        if (boardRoot == null ||
            towerRoot == null)
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

    public bool FinalizeStage()
    {
        if (!CanFinalizeStage || HasFinalizedStage)
        {
            return false;
        }

        // 各種認識を切る
        boundaryPlaneManager.enabled = false;
        markerTracking.enabled = false;

        // 現在の境界頂点の情報で上書きする
        finalizedBoundaryPoints.Clear();
        finalizedBoundaryPoints.AddRange(boundaryPoints);

        finalizedBoardPosition = boardRoot.position;
        finalizedBoardRotation = boardRoot.rotation;
        finalizedTowerPosition = towerRoot.localPosition;

        // 床メッシュ生成
        if (stageMeshGenerator == null ||
            !stageMeshGenerator.Generate(finalizedBoundaryPoints))
        {
            Debug.LogWarning("Stage mesh generation failed.");
            return false;
        }

        Transform inGameTowerTransform = Instantiate(towerInGamePrefab, towerRoot, false).transform;
        inGameTowerTransform.rotation = markerTracking.MarkerTransform.rotation;

        HasFinalizedStage = true;
        return true;
    }
}
