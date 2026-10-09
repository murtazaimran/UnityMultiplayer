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
            Camera playerCamera = Camera.main;

            if (playerCamera == null)
                return;

            Vector3 shootDirection =
                playerCamera.transform.forward;

            RequestSpawnRpc(shootDirection);
        }
    }

    [Rpc(
        SendTo.Server,
        InvokePermission = RpcInvokePermission.Everyone
    )]
    private void RequestSpawnRpc(Vector3 shootDirection)
    {
        SpawnProjectile(shootDirection);
    }

    private void SpawnProjectile(Vector3 shootDirection)
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

        if (shootDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion projectileRotation =
            Quaternion.LookRotation(shootDirection);

        NetworkObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            projectileRotation
        );

        projectile.SpawnWithOwnership(
            NetworkManager.ServerClientId
        );

        Debug.Log(
            $"Server spawned projectile from " +
            $"Player {OwnerClientId}"
        );
    }
}