using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        if (!IsOwner)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Camera playerCamera = Camera.main;

        if (playerCamera == null)
            return;

        // Get camera directions.
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Keep movement horizontal.
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Build movement relative to camera.
        Vector3 movement =
            cameraForward * vertical +
            cameraRight * horizontal;

        if (movement.sqrMagnitude > 1f)
            movement.Normalize();

        transform.position +=
            movement * moveSpeed * Time.deltaTime;
    }
}