using TMPro;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] ReticleManager reticleManager;

    public void LaunchProjectile()
    {
        var projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.Initialize(reticleManager.AimPosition);
    }
}
