
using UnityEngine;

public class HealthBehaviour : MonoBehaviour
{
    [SerializeField]
    private int startingHealth = 100;

    public HealthSystem Health { get; private set; }

    private void Awake()
    {
        Health = new HealthSystem(startingHealth);
    }

    public void ApplyDamage(int amount)
    {
        if (Health == null)
        {
            Debug.LogError(
                "HealthSystem has not been initialized.",
                this
            );
            return;
        }

        Health.TakeDamage(amount);
    }
}
