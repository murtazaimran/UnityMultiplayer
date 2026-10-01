using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBarUI healthBarUI;

    private NetworkVariable<int> currentHealth =
        new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        UpdateHealthBar();
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(
        int previousHealth,
        int newHealth)
    {
        Debug.Log(
            $"{gameObject.name} health: {newHealth}"
        );

        UpdateHealthBar();

        if (newHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarUI == null)
            return;

        healthBarUI.SetHealth(
            currentHealth.Value,
            maxHealth
        );
    }

    public void TakeDamage(int damage)
    {
        // Only the server can apply damage.
        if (!IsServer)
            return;

        currentHealth.Value = Mathf.Max(
            0,
            currentHealth.Value - damage
        );

        if (currentHealth.Value <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Only the server should despawn
        // the networked player.
        if (!IsServer)
            return;

        if (!IsSpawned)
            return;

        NetworkObject.Despawn();
    }
}