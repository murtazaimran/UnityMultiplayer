using Unity.Netcode;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        UI.SetActive(false);
    EnableCameraScript();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        UI.SetActive(false);
        EnableCameraScript();
    }

    void EnableCameraScript()
    {
        ThirdPersonCamera cameraScript = FindAnyObjectByType<ThirdPersonCamera>();
        if (cameraScript != null)
        {
            cameraScript.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}