using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public new Camera camera;

    public int id;
    public string username;
    public int currentHealth;
    public int maxHealth;

    public ParticleSystem damageEffect;
    public ParticleSystem healEffect;

    public Vector3 spawnLocation;
    public float respawnDelay;

    enum DamageReasons
    {
        OutOfBounds,
        Entity,

    }

    [Header("Movement")]
    public int acceleration;
    public int counterAcceleration;
    public float crouchMaxSpeed;
    public float walkMaxSpeed;
    public float runMaxSpeed;
    public bool isRunning;

    public float maxSpeed;
    public bool canMoveFAndB = true;
    public bool canMoveLAndR = true;

    public virtual void Initialize(int newId, string newUsername)
    {
        id = newId;
        username = newUsername;
        Server.entities.Add(id, this);
        maxSpeed = walkMaxSpeed;
    }

    public virtual void Damage(int amount, int damageDealerId)
    {
        currentHealth -= Mathf.Clamp(amount, 0, maxHealth - currentHealth);
        //damageEffect.Play();

        if (currentHealth <= 0)
        {
            Die();
        }

        ServerSend.EntityHealth(this);
    }

    public virtual void Heal(int amount)
    {
        currentHealth += Mathf.Clamp(amount, 0, maxHealth);
        //healEffect.Play();

        ServerSend.EntityHealth(this);
    }

    public virtual void Die()
    {
        transform.position = spawnLocation;
        ServerSend.EntityPosition(this);

        StartCoroutine(Respawn());
    }

    public virtual IEnumerator Respawn()
    {
        canMoveFAndB = false;
        canMoveLAndR = false;

        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        canMoveFAndB = true;
        canMoveLAndR = true;
        ServerSend.EntityRespawned(this);
    }
}
