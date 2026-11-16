using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

/// <summary>
/// 画像マーカーを検出し、その位置にプレハブを生成するクラス
/// </summary>
public class MarkerTracking : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARTrackedImageManager trackedImageManager;

    [Header("Prefabs")]
    [SerializeField] GameObject trackedImagePrefab; // マーカー検出時に生成するPrefab

    public event Action<ARTrackedImage> MarkerUpdate;
    public Transform MarkerTransform { get; private set; }

    Transform prefabTransform;                      // 生成したPrefabのTransform情報

    void OnEnable()
    {
        // 画像検出時のイベント追加
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = true;
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
        }
    }

    void OnDisable()
    {
        // 画像検出時のイベント解除
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
            trackedImageManager.enabled = false;
        }

        if (prefabTransform != null)
        {
            Destroy(prefabTransform.gameObject);
        }
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
            UpdateMarker(image);
        }

        // 移動時
        foreach (var image in changes.updated)
        {
            UpdateMarker(image);
        }
    }

    /// <summary>
    /// マーカー更新に伴う、平面の更新処理
    /// </summary>
    /// /// <param name="image">検出されたマーカー情報</param>
    void UpdateMarker(ARTrackedImage image)
    {
        // マーカーの追跡中でなければ、return
        if (image.trackingState != TrackingState.Tracking)
        {
            return;
        }

        MarkerTransform = image.transform;

        // マーカーの更新を通知
        MarkerUpdate?.Invoke(image);

        SetPrefab(image.transform);
    }

    /// <summary>
    /// マーカーに対してPrefabを生成、追従させる処理
    /// </summary>
    /// <param name="markerTransform">検出されたマーカーのTransform</param>
    void SetPrefab(Transform markerTransform)
    {
        // 一度も生成されていなければ、Prefabを生成
        if (prefabTransform == null)
        {
            prefabTransform = Instantiate(trackedImagePrefab).transform;
        }

        prefabTransform.SetPositionAndRotation(
            markerTransform.position,
            markerTransform.rotation
        );
    }
}
