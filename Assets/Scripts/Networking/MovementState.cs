using Unity.Netcode;
using UnityEngine;

public struct MovementState : INetworkSerializable
{
    public Vector3 Position;
    public int LastProcessedInput;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref LastProcessedInput);
    }
}