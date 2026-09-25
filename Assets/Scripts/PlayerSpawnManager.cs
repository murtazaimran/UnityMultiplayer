using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private float spawnOffset = 2f;

    private int nextSpawnIndex = 0;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        NetworkObject player = NetworkManager.Singleton.SpawnManager
            .GetPlayerNetworkObject(clientId);

        if (player == null)
            return;

        Vector3 spawnPosition = new Vector3(
            nextSpawnIndex * spawnOffset,
            0f,
            0f
        );

        player.transform.position = spawnPosition;

        nextSpawnIndex++;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}