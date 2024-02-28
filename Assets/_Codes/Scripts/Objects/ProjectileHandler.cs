using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            DestroyProjectile();
        }
        if (other.gameObject.CompareTag("LaserBlockObj"))
        {
            Debug.Log("Laser Block Object Hit!");
            DestroyProjectile();
        }
    }

    void DestroyProjectile(float seconds = 0)
    {
        var projectile = gameObject;
        Destroy(projectile, seconds);
    }
}
