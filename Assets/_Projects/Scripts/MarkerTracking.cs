using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// 画像マーカーを検出し、その位置にプレハブを生成するクラス
/// </summary>
public class MarkerTracking : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARTrackedImageManager trackedImageManager;

    [Header("Prefabs")]
    [SerializeField] GameObject trackedImagePrefab; // マーカー検出時に生成するPrefab

    Transform prefabTransform;                      // 生成したPrefabのTransform情報

    void OnEnable()
    {
        // 画像検出時のイベント購読
        trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
    }

    void OnDisable()
    {
        //イベント購読の解除
        trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    /// <summary>
    /// 登録されたマーカーの状態が変化した時に呼ばれる処理
    /// </summary>
    /// <param name="changes">状態変化の種類</param>
    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
    {
        // 検出時
        foreach (var image in changes.added)
        {
            SetPrefab(image);
        }

        // 移動時
        foreach (var image in changes.updated)
        {
            SetPrefab(image);
        }
    }

    /// <summary>
    /// マーカーに対してPrefabを生成、追従させる処理
    /// </summary>
    /// <param name="image">検出されたマーカー</param>
    void SetPrefab(ARTrackedImage image)
    {
        // マーカーの追跡中でなければ、return
        if (image.trackingState != TrackingState.Tracking)
        {
            return;
        }

        // 一度も生成されていなければ、Prefabを生成
        if (prefabTransform == null)
        {
            prefabTransform = Instantiate(trackedImagePrefab).transform;
        }

        prefabTransform.SetPositionAndRotation(
            image.transform.position,
            image.transform.rotation
        );
    }
}
