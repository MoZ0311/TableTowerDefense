using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] ReticleManager reticleManager;

    public void LaunchProjectile()
    {
        if (!reticleManager.HasTarget)
        {
            return;
        }
        var projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.Initialize(reticleManager.AimPosition);
    }
}
