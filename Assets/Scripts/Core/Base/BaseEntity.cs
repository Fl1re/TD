using System;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour,IDamageable
{
    [SerializeField] protected float maxHealth;
    private float _currentHealth;

    public float Health => _currentHealth;
    public float MaxHealth => maxHealth;
    public event Action OnDeath;
    public event Action OnHealthChange;

    public virtual void Initialize()
    {
        _currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        OnHealthChange?.Invoke();
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
