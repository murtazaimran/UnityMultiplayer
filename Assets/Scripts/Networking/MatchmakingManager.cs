
using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    private ISession currentSession;

    private Task initializationTask;

    public string JoinCode =>
        currentSession != null ? currentSession.Code : "";

    public bool HasSession => currentSession != null;

    private Task InitializeServices()
    {
        if (initializationTask == null)
        {
            initializationTask = InitializeServicesInternal();
        }

        return initializationTask;
    }

    private async Task InitializeServicesInternal()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance
                .SignInAnonymouslyAsync();
        }

        Debug.Log("Unity Services initialized successfully.");
        Debug.Log(
            $"Player ID: {AuthenticationService.Instance.PlayerId}"
        );
    }

    public async Task<bool> CreateSession()
    {
        try
        {
            await InitializeServices();

            var options = new SessionOptions
            {
                MaxPlayers = 2,
                Name = "Shooter Match"
            }.WithRelayNetwork();

            currentSession =
                await MultiplayerService.Instance
                    .CreateSessionAsync(options);

            Debug.Log($"Session created: {currentSession.Id}");
            Debug.Log($"Join code: {currentSession.Code}");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create session: {e}");
            return false;
        }
    }

    public async Task<bool> JoinSession(string code)
    {
        try
        {
            await InitializeServices();

            currentSession =
                await MultiplayerService.Instance
                    .JoinSessionByCodeAsync(code.Trim());

            Debug.Log($"Joined session: {currentSession.Id}");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join session: {e}");
            return false;
        }
    }

    public async Task LeaveSession()
    {
        if (currentSession == null)
            return;

        try
        {
            await currentSession.LeaveAsync();
            currentSession = null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to leave session: {e}");
        }
    }
}