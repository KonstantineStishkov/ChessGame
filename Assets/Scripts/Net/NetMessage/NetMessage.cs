using Assets.Scripts.Net;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetMessage
{
    public OpCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }
    public virtual void Deserialize(DataStreamReader reader)
    {

    }
    public virtual void ReceivedOnClient()
    {

    }
    public virtual void ReceivedOnServer(NetworkConnection connection)
    {

    }
}
