using Unity.Netcode;
using UnityEngine;

public struct PlayerInputData : INetworkSerializable
{
    public Vector2 MoveInput;
    public int SequenceNumber;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref MoveInput);
        serializer.SerializeValue(ref SequenceNumber);
    }
}