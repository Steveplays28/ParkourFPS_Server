using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;
    public string username = "";

    private float maxHealth = 100;
    public float health;

    public void Initialize(int _id, string _username, Vector3 _spawnPosition, Quaternion _spawnRotation)
    {
        id = _id;
        username = _username;

        transform.position = _spawnPosition;
        transform.rotation = _spawnRotation;

        health = maxHealth;
    }
}
