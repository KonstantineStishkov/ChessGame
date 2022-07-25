using Assets.Scripts.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public static class NetUtility
{
    public static void OnData(DataStreamReader reader, NetworkConnection connection, Server server = null)
    {
        NetMessage msg = null;
        var opCode = (OpCode)reader.ReadByte();

        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: 
                msg = new NetKeepAlive(reader); 
                break;
            case OpCode.WELCOME:
                msg = new NetWelcome(reader);
                break;
            case OpCode.START_GAME:
                msg = new NetStartGame(reader);
                break;
            case OpCode.MAKE_MOVE:
                msg = new NetMakeMove(reader);
                break;
            case OpCode.REMATCH:
                msg = new NetRematch(reader);
                break;
            default:
                Debug.LogError("Message received had no OpCode");
                break;
        }

        if (server == null)
            msg.ReceivedOnClient();
        else
            msg.ReceivedOnServer(connection);
    }
    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;
}
