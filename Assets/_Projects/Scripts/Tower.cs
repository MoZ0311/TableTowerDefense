using UnityEngine;
using System;
using DG.Tweening;

public class Tower : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] GameObject towerVisual;
    [SerializeField] ParticleSystem fireEffect;

    [Header("Shake")]
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float shakeStrength = 0.5f;
    [SerializeField] int vibrato = 20;
    [SerializeField] float randomness = 90;
    float currentHealth;
    public float HealthRatio { get => currentHealth / maxHealth; }

    public Action OnTowerDestroyed;

    void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        // ヒット時のシェイク
        towerVisual.transform.DOComplete();
        towerVisual.transform.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness, false, true);

        // 0以下にならないように調整
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        fireEffect.Play();
        OnTowerDestroyed?.Invoke();
    }
}
