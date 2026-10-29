using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class BoardPlacement : MonoBehaviour
{
    [SerializeField] BoardManager boardManager;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject boardPrefab;

    static readonly List<ARRaycastHit> raycastHits = new();

    GameObject placedBoard;

    void Update()
    {
        if (placedBoard != null)
        {
            return;
        }

        var touch = Touchscreen.current.primaryTouch;

        // タッチされたフレームのみ処理を行う
        if (touch.press.wasPressedThisFrame)
        {
            // タッチされた場所をもとに、検出平面を対象にRaycast
            Vector2 touchPosition = touch.position.ReadValue();
            if (raycastManager.Raycast(touchPosition, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                // Hitした場合、そこにBoardプレハブを作成
                Pose hitPose = raycastHits[0].pose;
                placedBoard = Instantiate(boardPrefab, hitPose.position, hitPose.rotation);
            }

            boardManager.DisableARPlaneManager();
        }
    }
}
