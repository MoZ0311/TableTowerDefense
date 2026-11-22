using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth;
    protected Transform target;
    float currentHealth;

    void OnEnable()
    {
        currentHealth = maxHealth;
        var tower = FindAnyObjectByType<Tower>();
        target = tower.transform;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
