using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] ReticleManager reticleManager;

    public bool CanFire { get => reticleManager.HasTarget; }

    public void LaunchProjectile()
    {
        if (!CanFire)
        {
            return;
        }
        var projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.Initialize(reticleManager.AimPosition);
    }
}
