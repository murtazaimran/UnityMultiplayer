
using Unity.Netcode;
using UnityEngine;

public class CoopObjective : NetworkBehaviour
{
    [SerializeField] private float interactionDistance = 3f;

    private NetworkVariable<bool> isActivated =
        new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public bool IsActivated => isActivated.Value;

    public override void OnNetworkSpawn()
    {
        isActivated.OnValueChanged += OnActivationChanged;
        UpdateVisuals(isActivated.Value);
    }

    public override void OnNetworkDespawn()
    {
        isActivated.OnValueChanged -= OnActivationChanged;
    }

    private void Update()
    {
        if (!IsSpawned || IsActivated || !IsClient)
            return;

        if (!IsOwnerOfLocalPlayer())
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestActivationRpc();
        }
    }

    private bool IsOwnerOfLocalPlayer()
    {
        NetworkObject[] players =
            FindObjectsByType<NetworkObject>(
                FindObjectsSortMode.None
            );

        foreach (NetworkObject player in players)
        {
            if (player.IsOwner &&
                Vector3.Distance(
                    player.transform.position,
                    transform.position
                ) <= interactionDistance)
            {
                return true;
            }
        }

        return false;
    }

    [Rpc(
        SendTo.Server,
        InvokePermission = RpcInvokePermission.Everyone
    )]
    private void RequestActivationRpc(
        RpcParams rpcParams = default)
    {
        if (isActivated.Value)
            return;

        ulong clientId = rpcParams.Receive.SenderClientId;

        if (!NetworkManager.ConnectedClients.TryGetValue(
                clientId,
                out NetworkClient client))
        {
            return;
        }

        if (client.PlayerObject == null)
            return;

        float distance = Vector3.Distance(
            client.PlayerObject.transform.position,
            transform.position
        );

        if (distance > interactionDistance)
        {
            Debug.Log(
                $"Client {clientId} is too far from the objective."
            );

            return;
        }

        isActivated.Value = true;

        Debug.Log(
            $"Objective activated by client {clientId}"
        );

        CoopGameManager gameManager =
            FindAnyObjectByType<CoopGameManager>();

        if (gameManager != null)
        {
            gameManager.CompleteObjective();
        }
        else
        {
            Debug.LogWarning("CoopGameManager was not found.");
        }
    }

    private void OnActivationChanged(
        bool previous,
        bool current)
    {
        UpdateVisuals(current);
    }

    private void UpdateVisuals(bool activated)
    {
        Renderer objectiveRenderer = GetComponent<Renderer>();

        if (objectiveRenderer != null)
        {
            objectiveRenderer.material.color =
                activated ? Color.green : Color.red;
        }
    }
}
