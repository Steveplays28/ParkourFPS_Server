using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public float damage;
    public float range;

    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.parent.forward, out hit, range))
        {
            Debug.DrawRay(transform.position, transform.parent.forward * hit.distance, Color.green, 20);

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<Player>().health -= damage;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.parent.forward * range, Color.red, 20);
        }
    }
}
