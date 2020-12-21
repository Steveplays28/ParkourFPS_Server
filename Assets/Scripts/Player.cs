using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public void Initialize(int _id, string _username, Vector3 _spawnPosition, Quaternion _spawnRotation)
    {
        id = _id;
        username = _username;

        transform.position = _spawnPosition;
        transform.rotation = _spawnRotation;
    }
}
