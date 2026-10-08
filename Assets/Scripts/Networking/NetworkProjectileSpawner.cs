using Unity.Netcode;
using UnityEngine;

public class NetworkProjectileSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestSpawnRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSpawnRpc()
    {
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        if (!IsServer)
            return;

        if (projectilePrefab == null)
        {
            Debug.LogError(
                "Projectile prefab is not assigned."
            );
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError(
                "Projectile spawn point is not assigned."
            );
            return;
        }

        NetworkObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        projectile.SpawnWithOwnership(
            NetworkManager.ServerClientId
        );

        Debug.Log(
            $"Server spawned projectile from " +
            $"Player {OwnerClientId} | " +
            $"Projectile Owner: {projectile.OwnerClientId}"
        );
    }
}