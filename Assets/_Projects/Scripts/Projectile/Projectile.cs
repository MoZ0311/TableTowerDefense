using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Vector3 targetPosition;

    public void Initialize(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
