using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int id;
    public int currentHealth;
    public int maxHealth;

    public ParticleSystem damageEffect;
    public ParticleSystem healEffect;

    public Vector3 spawnLocation;
    public float respawnDelay;

    public virtual void Damage(int amount)
    {
        currentHealth -= amount;
        damageEffect.Play();

        if (currentHealth <= 0)
        {
            Die();
        }

        ServerSend.EntityHealth(this);
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
        healEffect.Play();

        ServerSend.EntityHealth(this);
    }

    public virtual void Die()
    {
        transform.position = spawnLocation;
        ServerSend.EntityPosition(this);
    }

    public virtual IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        ServerSend.EntityRespawned(this);
    }
}
