
using System;

public class HealthSystem
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }

    public bool IsDead => CurrentHealth == 0;

    public HealthSystem(int maxHealth)
    {
        if (maxHealth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxHealth),
                "Maximum health must be greater than zero."
            );
        }

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead)
            return;

        CurrentHealth = Math.Max(0, CurrentHealth - damage);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead)
            return;

        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
    }
}
