using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;

public class BoundaryPlaneManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] LineRenderer fixedBoundaryLine;

    ARPlane selectedPlane;
    Transform boardRoot;

    Vector3[] fixedBoundary;
    bool isFinalized;

    public bool IsFinalized => isFinalized;
    public Vector3[] FixedBoundary => fixedBoundary;

    // MarkerTracking から、現在の候補平面と BoardRoot を受け取る
    public void SetCandidatePlane(ARPlane plane, Transform boardTransform)
    {
        if (isFinalized || plane == null || boardTransform == null)
        {
            return;
        }

        selectedPlane = plane;
        boardRoot = boardTransform;

        UpdatePlaneVisibility();
    }

    /// <summary>
    /// 選択した平面だけを描画する処理
    /// </summary>
    void UpdatePlaneVisibility()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            bool shouldEnable = plane == selectedPlane;
            plane.gameObject.SetActive(shouldEnable);
        }
    }

    /// <summary>
    /// ステージ外形の確定処理
    /// </summary>
    public void FinalizeStage()
    {
        if (isFinalized)
        {
            return;
        }

        if (selectedPlane == null || boardRoot == null)
        {
            Debug.LogWarning("ステージ確定できません。マーカーと机の平面を認識してください。");
            return;
        }

        NativeArray<Vector2> boundary = selectedPlane.boundary;

        if (!boundary.IsCreated || boundary.Length < 3)
        {
            Debug.LogWarning("平面の境界がまだ十分に取得できていません。");
            return;
        }

        // 境界を BoardRoot のローカル座標にコピーする
        fixedBoundary = new Vector3[boundary.Length];

        for (int i = 0; i < boundary.Length; ++i)
        {
            Vector2 point = boundary[i];

            Vector3 worldPoint = selectedPlane.transform.TransformPoint(new Vector3(point.x, 0f, point.y));

            fixedBoundary[i] = boardRoot.InverseTransformPoint(worldPoint);
        }

        // 固定用 LineRenderer を設定する
        DrawFixedBoundary();

        // 境界の保存後に検出を終了する
        isFinalized = true;

        // 検出済み平面の描画を停止
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        planeManager.enabled = false;

        Debug.Log($"ステージ確定完了。境界頂点数: {fixedBoundary.Length}");
    }

    void DrawFixedBoundary()
    {
        if (fixedBoundaryLine == null || fixedBoundary == null)
        {
            Debug.LogWarning("固定境界用 LineRenderer が割り当てられていません。");
            return;
        }

        fixedBoundaryLine.useWorldSpace = false;
        fixedBoundaryLine.loop = true;
        fixedBoundaryLine.positionCount = fixedBoundary.Length;

        for (int i = 0; i < fixedBoundary.Length; i++)
        {
            fixedBoundaryLine.SetPosition(i, fixedBoundary[i]);
        }

        fixedBoundaryLine.enabled = true;
    }
}
