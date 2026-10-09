
using TMPro;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private MatchmakingManager matchmakingManager;

    [Header("UI References")]
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text sessionCodeText;

    private bool isBusy;

    public async void StartHost()
    {
        if (isBusy)
            return;

        isBusy = true;
        SetStatus("Creating session...");

        bool success = await matchmakingManager.CreateSession();

        if (success)
        {
            if (sessionCodeText != null)
                sessionCodeText.text =
                    "Join Code: " + matchmakingManager.JoinCode;

            SetStatus("Session created. Share the join code.");
            EnterGameplay();
        }
        else
        {
            SetStatus("Could not create session. Check the Console.");
        }

        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy)
            return;

        string code = joinCodeInput != null
            ? joinCodeInput.text.Trim()
            : "";

        if (string.IsNullOrWhiteSpace(code))
        {
            SetStatus("Please enter a session join code.");
            return;
        }

        isBusy = true;
        SetStatus("Joining session...");

        bool success = await matchmakingManager.JoinSession(code);

        if (success)
        {
            SetStatus("Joined session successfully.");
            EnterGameplay();
        }
        else
        {
            SetStatus("Could not join. Check the code and Console.");
        }

        isBusy = false;
    }

    private void EnterGameplay()
    {
        if (UI != null)
            UI.SetActive(false);

        ThirdPersonCamera cameraScript =
            FindAnyObjectByType<ThirdPersonCamera>();

        if (cameraScript != null)
            cameraScript.enabled = true;
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}
