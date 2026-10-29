using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerTracking : MonoBehaviour
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARTrackedImageManager trackedImageManager;
    [SerializeField] GameObject boardPrefab;

    GameObject placedBoard;

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
            SetBoardPrefab(image);
        }

        foreach (var image in changes.updated)
        {
            SetBoardPrefab(image);
        }
    }

    void SetBoardPrefab(ARTrackedImage image)
    {
        // 画像の追跡中でなければ、return
        if (image.trackingState != TrackingState.Tracking)
        {
            return;
        }

        if (placedBoard == null)
        {
            placedBoard = Instantiate(boardPrefab);

            // Plane非表示
            // boardManager.DisableARPlaneManager();
        }

        UpdateBoardPose(image.transform);
    }

    void UpdateBoardPose(Transform targetTransform)
    {
        if (placedBoard == null)
        {
            return;
        }

        placedBoard.transform.SetPositionAndRotation(
            targetTransform.position,
            targetTransform.rotation
        );
    }
}
