using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBarUI healthBarUI;
    [SerializeField] private DeathUI deathUI;

    private NetworkVariable<int> currentHealth =
        new NetworkVariable<int>();

    void Awake()
    {
      if(deathUI == null)
      {
        deathUI = FindObjectOfType<DeathUI>()?.GetComponent<DeathUI>();
        }
    }
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

        // Only the player who owns this object
        // should see the death screen.
        if (newHealth <= 0 && IsOwner)
        {
            ShowDeathScreen();
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

    private void ShowDeathScreen()
    {
        if (deathUI == null)
        {
            Debug.LogWarning(
                "DeathUI is not assigned on PlayerHealth."
            );

            return;
        }

        deathUI.ShowDeathScreen();
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
        // Only the server can despawn
        // the networked player.
        if (!IsServer)
            return;

        if (!IsSpawned)
            return;

        NetworkObject.Despawn();
    }

  
}