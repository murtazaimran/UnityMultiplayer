using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetHealth(int currentHealth, int maxHealth)
    {
        float healthPercentage =
            (float)currentHealth / maxHealth;

        fillImage.fillAmount = healthPercentage;

        fillImage.color = Color.Lerp(
            Color.red,
            Color.green,
            healthPercentage
        );
    }
}