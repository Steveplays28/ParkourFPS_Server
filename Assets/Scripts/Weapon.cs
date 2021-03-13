using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Entity entity;

    public Mesh model;
    public Mesh barrel;
    public Mesh grip;
    public Mesh optic;
    public Mesh magazine;

    public Transform bulletOrigin;

    [Header("Weapon stats")]
    public string weaponName;
    public int damage;
    public float range;
    public int currentAmmo;
    public int maxAmmo;
    public int ammoPerShot;
    public float reloadTime;
    public bool isReloading;
    public float timeBetweenShots;
    public bool shooting;
    public bool isAutomatic;

    [Header("Animations")]
    public AnimationClip idleAnim;
    public AnimationClip shootAnim;
    public AnimationClip reloadAnim;

    private void Start()
    {
        //Instantiate(barrel, );
    }

    public void Shoot()
    {
        if (entity.currentHealth <= 0f || currentAmmo <= 0)
        {
            return;
        }

        if (shooting == false)
        {
            shooting = true;
        }

        if (Physics.Raycast(entity.transform.position, entity.transform.forward, out RaycastHit hit, range))
        {
            if (hit.collider.gameObject.GetComponent<Entity>() != null)
            {
                hit.collider.gameObject.GetComponent<Entity>().Damage(damage);
            }
        }
        currentAmmo -= ammoPerShot;

        if (isAutomatic && shooting)
        {
            StartCoroutine(ShootAgainAfterDelay());
        }
    }

    public IEnumerator ShootAgainAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        if (shooting)
        {
            Shoot();
        }
    }

    public void StopShooting()
    {
        shooting = false;
    }

    public IEnumerator Reload()
    {
        if (entity.currentHealth <= 0f || currentAmmo == maxAmmo)
        {
            yield break;
        }

        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        Debug.Log("haha reload go brrrrr");
    }

    public void StopReload()
    {
        StopCoroutine(Reload());
    }

    public void ResetWeapon()
    {
        currentAmmo = maxAmmo;
    }
}
