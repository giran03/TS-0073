using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            DestroyProjectile();

        if (other.gameObject.CompareTag("LaserBlockObj"))
            DestroyProjectile();
    }

    void DestroyProjectile(float seconds = 0)
    {
        var projectile = gameObject;
        Destroy(projectile, seconds);
    }
}
