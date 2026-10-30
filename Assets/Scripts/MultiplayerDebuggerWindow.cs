
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerDebuggerWindow : EditorWindow
{
    [MenuItem("Tools/Multiplayer Debugger")]
    public static void ShowWindow()
    {
        GetWindow<MultiplayerDebuggerWindow>(
            "Multiplayer Debugger"
        );
    }

    private void OnEnable()
    {
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Multiplayer Debugger",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        NetworkManager manager = NetworkManager.Singleton;

        if (manager == null)
        {
            EditorGUILayout.HelpBox(
                "NetworkManager not found. Make sure it exists in the active scene.",
                MessageType.Warning
            );

            return;
        }

        if (!manager.IsListening)
        {
            EditorGUILayout.HelpBox(
                "Multiplayer is currently stopped. Start a Host or Client to view diagnostics.",
                MessageType.Info
            );
        }

        string role = manager.IsHost ? "Host" :
                      manager.IsServer ? "Server" :
                      manager.IsClient ? "Client" : "None";

        EditorGUILayout.LabelField(
            "Network Status",
            manager.IsListening ? "Connected / Listening" : "Stopped"
        );

        EditorGUILayout.LabelField(
            "Network Role",
            role
        );

        EditorGUILayout.LabelField(
            "Connected Players",
            manager.ConnectedClients.Count.ToString()
        );

        EditorGUILayout.Space();

        GUILayout.Label(
            "Connected Player Details",
            EditorStyles.boldLabel
        );

        foreach (var client in manager.ConnectedClientsList)
        {
            string playerName = client.PlayerObject != null
                ? client.PlayerObject.name
                : "No PlayerObject";

            string position = client.PlayerObject != null
                ? client.PlayerObject.transform.position.ToString("F2")
                : "N/A";

            EditorGUILayout.LabelField(
                $"Client ID: {client.ClientId}",
                playerName
            );

            EditorGUILayout.LabelField(
                "Position",
                position
            );

            EditorGUILayout.Space(2);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Copy Diagnostics"))
        {
            CopyDiagnostics(manager, role);
        }
    }

    private void CopyDiagnostics(
        NetworkManager manager,
        string role)
    {
        StringBuilder report = new StringBuilder();

        report.AppendLine("MULTIPLAYER DIAGNOSTICS");
        report.AppendLine("----------------------");

        report.AppendLine(
            $"Network Status: {(manager.IsListening ? "Listening" : "Stopped")}"
        );

        report.AppendLine($"Network Role: {role}");

        report.AppendLine(
            $"Connected Players: {manager.ConnectedClients.Count}"
        );

        report.AppendLine();
        report.AppendLine("PLAYER DETAILS");

        foreach (var client in manager.ConnectedClientsList)
        {
            report.AppendLine(
                $"Client ID: {client.ClientId}"
            );

            if (client.PlayerObject != null)
            {
                report.AppendLine(
                    $"Position: {client.PlayerObject.transform.position:F2}"
                );
            }
            else
            {
                report.AppendLine("Position: N/A");
            }
        }

        EditorGUIUtility.systemCopyBuffer = report.ToString();

        Debug.Log(
            "Multiplayer diagnostics copied to clipboard."
        );
    }
}
