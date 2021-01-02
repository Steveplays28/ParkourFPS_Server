using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Player player;

    [Header("Weapon stats")]
    public string weaponName;
    public float damage;
    public float range;

    [Header("Reloadable weapons only")]
    public bool usesAmmo = true;
    public int currentAmmo;
    public int maxAmmo;
    public int ammoPerShot;
    public float reloadTime;
    public bool isReloading;

    [Header("Automatic weapons only")]
    public bool isAutomatic;
    public float timeBetweenShots;
    public bool isShooting;
    public bool isWaiting;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void Shoot()
    {
        if (player.health <= 0f)
        {
            return;
        }
        else
        {
            if (isAutomatic && isShooting)
            {
                isShooting = false;
                return;
            }
        }

        if (isReloading)
        {
            return;
        }
        else if (currentAmmo <= 0 && usesAmmo)
        {
            Reload();
            return;
        }

        if (isAutomatic && isShooting == true)
        {
            isShooting = false;
            return;
        }

        if (isAutomatic && isShooting == false)
        {
            isShooting = true;
        }   

        ServerSend.PlayerShoot(player.id, player);

        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out RaycastHit _hit, range))
        {
            if (usesAmmo)
            {
                currentAmmo -= ammoPerShot;
            }

            if (_hit.collider.CompareTag("Player"))
            {
                if (_hit.collider.gameObject.GetComponent<Player>().id != player.id)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(damage);
                }
            }
            else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        if (isAutomatic)
        {
            StartCoroutine(ShootAfterDelay());
        }
    }

    public void ShootAutomatic()
    {
        if (player.health <= 0f || isReloading || isShooting == false || isWaiting)
        {
            return;
        }
        else if (currentAmmo <= 0 && usesAmmo)
        {
            Reload();
            return;
        }

        ServerSend.PlayerShoot(player.id, player);

        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out RaycastHit _hit, range))
        {
            if (usesAmmo)
            {
                currentAmmo -= ammoPerShot;
            }

            if (_hit.collider.CompareTag("Player"))
            {
                if (_hit.collider.gameObject.GetComponent<Player>().id != player.id)
                {
                    _hit.collider.GetComponent<Player>().TakeDamage(damage);
                }
            }
            else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        StartCoroutine(ShootAfterDelay());
    }

    private IEnumerator ShootAfterDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(timeBetweenShots);
        isWaiting = false;
        ShootAutomatic();

        yield break;
    }

    public void Reload()
    {
        if (currentAmmo == maxAmmo || player.health <= 0f || isReloading || usesAmmo == false)
        {
            return;
        }

        isReloading = true;
        ServerSend.PlayerReloadWeapon(player.id, player);
        StartCoroutine(ReloadAfterDelay());
    }

    private IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;

        if (isAutomatic)
        {
            ShootAutomatic();
        }

        yield break;
    }

    public void CancelReload()
    {
        if (isReloading)
        {
            isReloading = false;
            StopCoroutine(ReloadAfterDelay());
        }
    }

    public void ResetWeapon()
    {
        CancelReload();
        currentAmmo = maxAmmo;
    }
}
