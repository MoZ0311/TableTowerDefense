using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class BoardAnchor : MonoBehaviour
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] ARTrackedImageManager trackedImageManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] GameObject boardPrefab;

    bool hasCreatedAnchor;

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

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
    {
        foreach (var image in changes.added)
        {
            TryCreateAnchorAsync(image);
        }

        foreach (var image in changes.updated)
        {
            TryCreateAnchorAsync(image);
        }
    }

    async void TryCreateAnchorAsync(ARTrackedImage image)
    {
        // 既にアンカーを設定していれば、return
        if (hasCreatedAnchor)
        {
            // return;
        }

        // 画像の追跡中でなければ、return
        if (image.trackingState != TrackingState.Tracking)
        {
            return;
        }

        try
        {
            // 画像マーカーの位置を基準にアンカーを設定
            Pose pose = image.pose;
            var result = await anchorManager.TryAddAnchorAsync(pose);

            // アンカーの作成に成功すれば、そこにボードを生成
            if (result.status.IsSuccess())
            {
                var anchor = result.value;

                var board = Instantiate(boardPrefab, anchor.transform, false);

                board.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                Debug.Log("boardをアンカーの子に設定");

                hasCreatedAnchor = true;

                boardManager.DisableARPlaneManager();
            }
        }
        catch
        {
            Debug.LogError("アンカーの設定に失敗");
            hasCreatedAnchor = false;
        }
    }
}
