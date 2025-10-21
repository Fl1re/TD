using System;

public interface IDamageable
{
    float Health { get; }
    void TakeDamage(float damage);
    event Action OnDeath;
    event Action OnHealthChange;
}