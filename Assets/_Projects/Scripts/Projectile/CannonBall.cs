using UnityEngine;
using DG.Tweening;

public class CannonBall : Projectile
{
    [Header("Ball")]
    [SerializeField] float dulation;

    [Header("Explosion")]
    [SerializeField] float damageAmount;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] float explodeRadius;
    [SerializeField] LayerMask targetLayer;

    const int MaxColliderCount = 32;
    static readonly Collider[] hitColliders = new Collider[MaxColliderCount];
    Sequence cannonBallSequence;


    void OnEnable()
    {
        OnInitialized += ShootCannonBall;
    }

    void OnDisable()
    {
        OnInitialized -= ShootCannonBall;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Tower tower))
        {
            tower.TakeDamage(0);
            cannonBallSequence.Kill();
            Explode();
        }
    }

    void ShootCannonBall()
    {
        cannonBallSequence = DOTween.Sequence().SetLink(gameObject);
        cannonBallSequence.Append(transform.DOMove(targetPosition, dulation).SetEase(Ease.Linear))
                .AppendCallback(() => Explode());
    }

    void Explode()
    {
        // 指定した中心点と半径の球体内にあるコライダーをすべて取得
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explodeRadius, hitColliders, targetLayer);

        for (int i = 0; i < hitCount; ++i)
        {
            Collider hit = hitColliders[i];

            // ダメージ処理
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damageAmount);
            }
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
