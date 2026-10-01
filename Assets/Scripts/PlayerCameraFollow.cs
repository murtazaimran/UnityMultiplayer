using Unity.Netcode;
using UnityEngine;

public class PlayerCameraFollow : NetworkBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);

    private Camera mainCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        mainCamera.transform.position = transform.position + offset;
        mainCamera.transform.LookAt(transform);
    }

    private void LateUpdate()
    {
        if (!IsOwner || mainCamera == null)
            return;

        mainCamera.transform.position = transform.position + offset;
        mainCamera.transform.LookAt(transform);
    }
}