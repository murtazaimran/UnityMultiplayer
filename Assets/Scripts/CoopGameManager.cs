using Unity.Netcode;
using UnityEngine;

public class CoopGameManager : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject objectiveInstruction;

    private NetworkVariable<bool> hasWon =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public bool HasWon => hasWon.Value;

    public override void OnNetworkSpawn()
    {
        hasWon.OnValueChanged += OnWinStateChanged;
        UpdateUI(hasWon.Value);
    }

    public override void OnNetworkDespawn()
    {
        hasWon.OnValueChanged -= OnWinStateChanged;
    }

    public void CompleteObjective()
    {
        if (!IsServer || hasWon.Value)
            return;

        hasWon.Value = true;
        Debug.Log("Co-op objective complete! Players win!");
    }

    private void OnWinStateChanged(bool previous, bool current)
    {
        UpdateUI(current);
    }

    private void UpdateUI(bool won)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(won);

        if (objectiveInstruction != null)
            objectiveInstruction.SetActive(!won);

        if (won)
            Debug.Log("VICTORY! The team completed the objective.");
    }
}