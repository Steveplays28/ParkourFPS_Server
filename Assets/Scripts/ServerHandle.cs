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

    public static void Ping(int fromClient, Packet packet)
    {
        ServerSend.Ping(fromClient);
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
        if (Server.clients[_fromClient].player.weapon.shooting)
        {
            return;
        }
        else
        {
            Server.clients[_fromClient].player.weapon.Shoot();
        }
    }

    public static void PlayerStopShooting(int fromClient, Packet packet)
    {
        Server.clients[fromClient].player.weapon.StopShooting();
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

    public static void PlayerEquipWeapon(int fromClient, Packet packet)
    {
        int weaponId = packet.ReadInt();

        Server.clients[fromClient].player.EquipWeapon(weaponId);
    }

    public static void PlayerReloadWeapon(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.weapon.StartCoroutine("Reload");
        Debug.Log("reload pls");
    }
}
