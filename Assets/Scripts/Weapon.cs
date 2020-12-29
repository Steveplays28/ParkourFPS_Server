using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Player player;

    [Header("Weapon stats")]
    public string weaponName;
    public float damage;
    public bool usesAmmo = true;
    public int currentAmmo;
    public int maxAmmo;
    public int ammoPerShot;
    public float range;
    public float reloadTime;
    public bool isReloading;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void Shoot()
    {
        if (player.health <= 0f || isReloading)
        {
            return;
        }
        else if (currentAmmo <= 0 && usesAmmo)
        {
            Reload();
        }
        else
        {
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
        }
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
}
