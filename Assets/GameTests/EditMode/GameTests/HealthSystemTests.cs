
using NUnit.Framework;
using System;

public class HealthSystemTests
{
    [Test]
    public void Constructor_InitializesHealthToMaximum()
    {
        var health = new HealthSystem(100);

        Assert.AreEqual(100, health.MaxHealth);
        Assert.AreEqual(100, health.CurrentHealth);
        Assert.IsFalse(health.IsDead);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        var health = new HealthSystem(100);

        health.TakeDamage(20);

        Assert.AreEqual(80, health.CurrentHealth);
    }

    [Test]
    public void TakeDamage_DoesNotGoBelowZero()
    {
        var health = new HealthSystem(100);

        health.TakeDamage(150);

        Assert.AreEqual(0, health.CurrentHealth);
        Assert.IsTrue(health.IsDead);
    }

    [Test]
    public void TakeDamage_ZeroOrNegativeDamage_DoesNothing()
    {
        var health = new HealthSystem(100);

        health.TakeDamage(0);
        health.TakeDamage(-10);

        Assert.AreEqual(100, health.CurrentHealth);
    }

    [Test]
    public void Heal_DoesNotExceedMaximumHealth()
    {
        var health = new HealthSystem(100);
        health.TakeDamage(30);

        health.Heal(50);

        Assert.AreEqual(100, health.CurrentHealth);
    }

    [Test]
    public void Heal_DoesNothingWhenHealthIsZero()
    {
        var health = new HealthSystem(100);
        health.TakeDamage(100);

        health.Heal(50);

        Assert.AreEqual(0, health.CurrentHealth);
        Assert.IsTrue(health.IsDead);
    }

    [Test]
    public void Constructor_RejectsInvalidMaximumHealth()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new HealthSystem(0)
        );

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new HealthSystem(-10)
        );
    }
}
