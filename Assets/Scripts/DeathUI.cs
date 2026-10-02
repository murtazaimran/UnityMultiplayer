using UnityEngine;

public class DeathUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;

    private void Start()
    {
        deathPanel.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

      public void RestartGame()
    {
        // // Only the server can restart the game.
        // if (!IsServer)
        //     return;

        // currentHealth.Value = maxHealth;
        Application.LoadLevel(Application.loadedLevel);
    }
}