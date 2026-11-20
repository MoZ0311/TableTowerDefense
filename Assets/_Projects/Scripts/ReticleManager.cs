using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ReticleManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform reticleTransform;
    [SerializeField] GameObject reticleModel;

    static readonly List<ARRaycastHit> raycastHits = new(); // AR空間でのRaycastを格納するList

    public bool HasTarget { get; private set; }
    public Vector3 AimPosition { get; private set; }

    void Update()
    {
        // 画面中央からレイを飛ばす
        Vector3 screenCenter = mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        HasTarget = raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon);

        // hitしたかどうかで照準のActive切替
        reticleModel.SetActive(HasTarget);
        if (HasTarget)
        {
            // hitした場合、AimPositionを更新し、レティクルを動かす
            AimPosition = raycastHits[0].pose.position;
            reticleTransform.position = AimPosition;
        }
    }
}
