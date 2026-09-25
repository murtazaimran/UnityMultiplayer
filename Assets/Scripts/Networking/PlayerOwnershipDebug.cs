using Unity.Netcode;
using UnityEngine;

public class PlayerOwnershipDebug : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log(
            $"Player spawned | Owner: {OwnerClientId}"
        );
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log(
            $"Player despawned | Owner: {OwnerClientId}"
        );
    }
}