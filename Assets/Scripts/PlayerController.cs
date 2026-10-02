using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float reconciliationSpeed = 15f;

    private int nextSequenceNumber = 0;

    private readonly List<PlayerInputData> pendingInputs =
        new List<PlayerInputData>();

    private Vector3 reconciliationTarget;
    private bool isReconciling;

    private void Update()
    {
        if (!IsOwner)
            return;

        CaptureInput();
    }

    private void LateUpdate()
    {
        if (!IsOwner || !isReconciling)
            return;

        transform.position = Vector3.Lerp(
            transform.position,
            reconciliationTarget,
            reconciliationSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
                transform.position,
                reconciliationTarget) < 0.01f)
        {
            transform.position = reconciliationTarget;
            isReconciling = false;
        }
    }

    private void CaptureInput()
    {
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

        Vector2 input = new Vector2(
            movement.x,
            movement.z
        );

        PlayerInputData inputData = new PlayerInputData
        {
            MoveInput = input,
            SequenceNumber = nextSequenceNumber
        };

        nextSequenceNumber++;

        pendingInputs.Add(inputData);

        // Client-side prediction.
        ApplyMovement(inputData);

        // Send the same input to the server.
        SendInputServerRpc(inputData);
    }

    private void ApplyMovement(PlayerInputData inputData)
    {
        Vector3 movement = new Vector3(
            inputData.MoveInput.x,
            0f,
            inputData.MoveInput.y
        );

        transform.position +=
            movement * moveSpeed * Time.deltaTime;
    }

    [ServerRpc]
    private void SendInputServerRpc(PlayerInputData inputData)
    {
        ProcessMovement(inputData);
    }

    private void ProcessMovement(PlayerInputData inputData)
    {
        Vector3 movement = new Vector3(
            inputData.MoveInput.x,
            0f,
            inputData.MoveInput.y
        );

        transform.position +=
            movement * moveSpeed * Time.deltaTime;

        MovementState state = new MovementState
        {
            Position = transform.position,
            LastProcessedInput = inputData.SequenceNumber
        };

        SendMovementStateClientRpc(state);

        Debug.Log(
            $"Server processed input | " +
            $"Player: {OwnerClientId} | " +
            $"Sequence: {inputData.SequenceNumber}"
        );
    }

    [ClientRpc]
    private void SendMovementStateClientRpc(
        MovementState state)
    {
        if (IsOwner)
        {
            // Smoothly correct toward the
            // server-authoritative position.
            reconciliationTarget = state.Position;
            isReconciling = true;

            // Remove inputs already processed
            // by the server.
            pendingInputs.RemoveAll(
                input => input.SequenceNumber <=
                         state.LastProcessedInput
            );

            // Replay inputs that the server
            // has not processed yet.
            foreach (PlayerInputData input in pendingInputs)
            {
                ApplyMovement(input);
            }

            Debug.Log(
                $"Reconciled | " +
                $"Server Input: {state.LastProcessedInput} | " +
                $"Remaining Inputs: {pendingInputs.Count}"
            );
        }
        else
        {
            // Apply authoritative server position
            // for other players.
            transform.position = state.Position;
        }
    }
}