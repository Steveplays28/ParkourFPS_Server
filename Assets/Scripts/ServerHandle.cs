using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _playerRotation = _packet.ReadQuaternion();
        Quaternion _cameraRotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _playerRotation, _cameraRotation);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot();
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem();
    }

    public static void PlayerJump(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.Jump();
    }

    public static void PlayerRun(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.Run();
    }

    public static void PlayerCrouch(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.Crouch();
    }
}
