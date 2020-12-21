using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Vector3 position;
    public Quaternion rotation;

    public void Initialize(int _id, string _username, Vector3 _spawnPosition, Quaternion _spawnRotation)
    {
        id = _id;
        username = _username;

        position = _spawnPosition;
        rotation = _spawnRotation;
    }
}
