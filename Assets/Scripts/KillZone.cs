using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KillZone : MonoBehaviour
{
    public int damage = int.MaxValue;

    private void OnCollisionEnter(Collision collision)
    {
        Entity hitEntity = collision.gameObject.GetComponent<Entity>();

        if (hitEntity != null)
        {
            hitEntity.Damage(damage, hitEntity.id);
        }
    }
}
