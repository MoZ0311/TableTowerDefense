using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

/// <summary>
/// AR座標系をもとに、タップされた場所にプレハブを生成するクラス
/// </summary>
public class PrefabPlacement : MonoBehaviour
{
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject placementPrefab;            // 生成するPrefab

    static readonly List<ARRaycastHit> raycastHits = new(); // AR空間でのRaycastを格納するList
    Transform prefabTransform;                              // 生成したPrefabのTransform情報

    void Update()
    {
        // 生成後であれば、早期return
        if (prefabTransform != null)
        {
            return;
        }

        Vector2 pressedPosition;    // 押された場所
        bool wasPressedThisFrame;   // そのフレームで押されたか

        if (Application.isMobilePlatform)
        {
            // スクリーンのタッチ情報をもとに、タッチ座標とタッチした瞬間を取る
            var touch = Touchscreen.current.primaryTouch;
            pressedPosition = touch.position.ReadValue();
            wasPressedThisFrame = touch.press.wasPressedThisFrame;
        }
        else
        {
            // マウスのクリック情報をもとに、クリック座標とクリックした瞬間を取る
            var click = Mouse.current.leftButton;
            pressedPosition = Mouse.current.position.ReadValue();
            wasPressedThisFrame = click.wasPressedThisFrame;
        }

        // タッチorクリックされたフレームのみ処理を行う
        if (wasPressedThisFrame)
        {
            // 入力された場所をもとに、検出平面を対象にRaycast
            if (raycastManager.Raycast(pressedPosition, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                // Hitした場合、そこにBoardプレハブを作成
                Pose hitPose = raycastHits[0].pose;
                prefabTransform = Instantiate(placementPrefab, hitPose.position, hitPose.rotation).transform;
            }
        }
    }
}
