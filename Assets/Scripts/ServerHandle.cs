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
            Debug.LogError($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }

        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void UDPTestReceived(int _fromClient, Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log($"Received packet via UDP. Contains message: {_msg}");
    }

    public static void PlayerInput(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[11];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        Quaternion _mouseRotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.GetComponent<FpsController>().inputs = _inputs;
        Server.clients[_fromClient].player.GetComponent<FpsController>().mouseRotation = _mouseRotation;

        Server.clients[_fromClient].player.GetComponent<FpsController>().CustomUpdate();
    }
}
