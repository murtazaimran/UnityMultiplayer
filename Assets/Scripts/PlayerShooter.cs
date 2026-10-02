using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooter : NetworkBehaviour
{
    [SerializeField] private float shootRange = 500f;
    [SerializeField] private LineRenderer shotLine;
    [SerializeField] private float shotDuration = 0.1f;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera playerCamera = Camera.main;

            if (playerCamera == null)
                return;

            // Shoot a ray from the center of the camera.
            Ray cameraRay = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward
            );

            Vector3 aimPoint =
                cameraRay.origin +
                cameraRay.direction * shootRange;

            // Find exactly what the crosshair is pointing at.
            if (Physics.Raycast(
                cameraRay,
                out RaycastHit cameraHit,
                shootRange))
            {
                aimPoint = cameraHit.point;
            }

            // Send the point we're aiming at to the server.
            FireServerRpc(aimPoint);
        }
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 aimPoint)
    {
        // Server decides where the shot originates.
        Vector3 origin = transform.position + Vector3.up;

        // Calculate direction from the player toward
        // the point the client was aiming at.
        Vector3 direction =
            (aimPoint - origin).normalized;

        Vector3 shotEndPoint =
            origin + direction * shootRange;

        // Server performs the actual hit detection.
        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            shootRange))
        {
            shotEndPoint = hit.point;

            Debug.Log(
                $"Server: Player {OwnerClientId} hit " +
                $"{hit.collider.gameObject.name}"
            );

            // Prevent shooting yourself.
            if (hit.collider.transform == transform)
            {
                Debug.Log(
                    "Server: Player cannot shoot themselves."
                );

                ShowShotClientRpc(
                    origin,
                    shotEndPoint
                );

                return;
            }

            // Check if the object has PlayerHealth.
            PlayerHealth targetHealth =
                hit.collider.GetComponent<PlayerHealth>();

            if (targetHealth == null)
            {
                Debug.Log(
                    "Server: Hit object is not a valid player."
                );

                ShowShotClientRpc(
                    origin,
                    shotEndPoint
                );

                return;
            }

            // Server applies damage.
            targetHealth.TakeDamage(25);
        }
        else
        {
            Debug.Log(
                $"Server: Player {OwnerClientId} missed."
            );
        }

        // Show the shot to all clients.
        ShowShotClientRpc(
            origin,
            shotEndPoint
        );
    }

    [ClientRpc]
    private void ShowShotClientRpc(
        Vector3 start,
        Vector3 end)
    {
        StartCoroutine(
            ShowShot(start, end)
        );
    }

    private IEnumerator ShowShot(
        Vector3 start,
        Vector3 end)
    {
        shotLine.SetPosition(0, start);
        shotLine.SetPosition(1, end);

        shotLine.enabled = true;

        yield return new WaitForSeconds(
            shotDuration
        );

        shotLine.enabled = false;
    }
}