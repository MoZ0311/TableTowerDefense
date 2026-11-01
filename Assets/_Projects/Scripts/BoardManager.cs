using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BoardManager : MonoBehaviour
{
    [SerializeField] ARPlaneManager planeManager;

    public void DisableARPlaneManager()
    {
        // 平面検出機能をオフにする
        planeManager.enabled = false;

        // 検出平面を一括で隠す
        DisablePlanes();
    }

    void DisablePlanes()
    {
        // 全ての検出済みPlaneを無効化
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }
}
