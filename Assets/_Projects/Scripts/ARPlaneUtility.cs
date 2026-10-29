using UnityEngine;
using UnityEngine.XR.ARFoundation;

public static class ARPlaneUtility
{
    /// <summary>
    /// 平面検出を停止し、全ての検出済みPlaneを無効化する処理
    /// </summary>
    public static void DisableManagerAndPlanes()
    {
        // 現在のシーン上から検索
        ARPlaneManager planeManager = Object.FindAnyObjectByType<ARPlaneManager>();

        // 各関数を引数付で実行
        DisableManager(planeManager);
        DisableAllPlanes(planeManager);
    }

    /// <summary>
    /// 全ての検出済み平面を無効化する処理
    /// </summary>
    /// <param name="targetManager"></param>
    public static void DisableAllPlanes(ARPlaneManager targetManager = null)
    {
        if (targetManager == null)
        {
            // 引数なしの時は現在のシーン上から検索
            targetManager = Object.FindAnyObjectByType<ARPlaneManager>();
        }

        // 全てのARPlaneに対してSetActive()を実行
        targetManager.SetTrackablesActive(false);
    }

    /// <summary>
    /// 平面検出を停止する処理
    /// </summary>
    /// <param name="targetManager"></param>
    public static void DisableManager(ARPlaneManager targetManager = null)
    {
        if (targetManager == null)
        {
            // 引数なしの時は現在のシーン上から検索
            targetManager = Object.FindAnyObjectByType<ARPlaneManager>();
        }

        // コンポーネントを無効化
        targetManager.enabled = false;
    }
}
