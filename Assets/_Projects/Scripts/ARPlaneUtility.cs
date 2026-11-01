using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// ARPlaneManagerに関する処理をまとめた静的クラス
/// </summary>
public static class ARPlaneUtility
{
    /// <summary>
    /// 平面検出を停止し、全ての検出済みPlaneを無効化する処理
    /// </summary>
    public static void StopDetectionAndHidePlanes(ARPlaneManager targetManager)
    {
        // ARPlaneManagerのnullチェック
        if (targetManager == null)
        {
            Debug.LogError("targetManager is null");
            return;
        }

        // 各関数を実行
        DisableManager(targetManager);
        HideAllPlanes(targetManager);
    }

    /// <summary>
    /// 全ての検出済み平面を非表示にする処理
    /// </summary>
    /// <param name="targetManager">対象のARPlaneManager</param>
    public static void HideAllPlanes(ARPlaneManager targetManager)
    {
        // ARPlaneManagerのnullチェック
        if (targetManager == null)
        {
            Debug.LogError("targetManager is null");
            return;
        }

        // trackables(認識したARPlane)を順番に処理
        foreach (var plane in targetManager.trackables)
        {
            // ARPlaneコンポーネントを持つオブジェクトの、描画用コンポーネントを無効化
            if (plane.TryGetComponent<ARPlaneMeshVisualizer>(out var visualizer))
            {
                visualizer.enabled = false;
            }
            else
            {
                Debug.LogWarning($"could not find ARPlaneMeshVisualizer");
            }
        }
    }

    /// <summary>
    /// 全ての検出済み平面を無効化する処理
    /// </summary>
    /// <param name="targetManager">対象のARPlaneManager</param>
    public static void DisableAllPlanes(ARPlaneManager targetManager)
    {
        // ARPlaneManagerのnullチェック
        if (targetManager == null)
        {
            Debug.LogError("targetManager is null");
            return;
        }

        // 全てのARPlaneに対してSetActive()を実行
        targetManager.SetTrackablesActive(false);
    }

    /// <summary>
    /// 平面検出を停止する処理
    /// </summary>
    /// <param name="targetManager">対象のARPlaneManager</param>
    public static void DisableManager(ARPlaneManager targetManager)
    {
        // ARPlaneManagerのnullチェック
        if (targetManager == null)
        {
            Debug.LogError("targetManager is null");
            return;
        }

        // コンポーネントを無効化
        targetManager.enabled = false;
    }
}
