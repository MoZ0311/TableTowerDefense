using UnityEngine;
using System;

public abstract class Projectile : MonoBehaviour
{
    protected Action OnInitialized;
    protected Vector3 targetPosition;

    public void Initialize(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        OnInitialized?.Invoke();
    }
}
