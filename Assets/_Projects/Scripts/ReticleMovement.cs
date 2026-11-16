using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ReticleMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARRaycastManager raycastManager;

    [SerializeField] Camera mainCamera;

    [SerializeField] Transform reticleTransform;
    [SerializeField] GameObject reticleModel;

    static readonly List<ARRaycastHit> raycastHits = new(); // AR空間でのRaycastを格納するList

    void Update()
    {
        // 画面中央からレイを飛ばす
        var screenCenter = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        bool hasHit = raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon);
        reticleModel.SetActive(hasHit);
        if (hasHit)
        {
            // hitした場合、レティクルを動かす
            reticleTransform.position = raycastHits[0].pose.position;
        }
    }
}
