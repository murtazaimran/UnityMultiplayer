using Unity.Netcode;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    private float lifetimeTimer;

    private void Update()
    {
        if (!IsServer)
            return;

        // Move the projectile on the server.
        transform.position +=
            transform.forward * speed * Time.deltaTime;

        // Track projectile lifetime.
        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= lifetime)
        {
            DespawnProjectile();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer)
            return;

        Debug.Log(
            $"Projectile collided with: {collision.gameObject.name}"
        );

        DespawnProjectile();
    }

    private void DespawnProjectile()
    {
        if (!IsSpawned)
            return;

        NetworkObject.Despawn();
    }
}