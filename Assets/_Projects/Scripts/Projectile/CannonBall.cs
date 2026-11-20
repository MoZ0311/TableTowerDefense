using UnityEngine;
using DG.Tweening;

public class CannonBall : Projectile
{
    [Header("Ball")]
    [SerializeField] float jumpForce;
    [SerializeField] float dulation;

    [Header("Explosion")]
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] float explodeRadius;
    [SerializeField] LayerMask targetLayer;

    const int MaxColliderCount = 32;

    static readonly Collider[] hitColliders = new Collider[MaxColliderCount];


    void OnEnable()
    {
        OnInitialized += ShootCannonBall;
    }

    void OnDisable()
    {
        OnInitialized -= ShootCannonBall;
    }

    void ShootCannonBall()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPosition, dulation).SetEase(Ease.Linear))
                .AppendCallback(() => Explode());
    }

    void Explode()
    {
        Debug.Log("Explode!");

        // 指定した中心点と半径の球体内にあるコライダーをすべて取得
        Physics.OverlapSphereNonAlloc(transform.position, explodeRadius, hitColliders, targetLayer);

        foreach (Collider hit in hitColliders)
        {
            // ダメージ処理
        }

        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
