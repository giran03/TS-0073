using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject projectileObj;
    [SerializeField] Transform projectileOrigin;
    public float projectileForce = 12;
    public float turretRayDistance = 15;
    public float rotationSpeed = 100;
    public float fireRate = .1f;
    private float nextFireTime; // The next time the turret will fire

    GameObject projectile;

    void Update()
    {
        // Check if the player is in sight
        if (PlayerInSight())
        {
            // Rotate the turret to face the player
            RotateTowardsPlayer();

            if (Time.time > nextFireTime)
            {
                FireProjectile();

                nextFireTime = Time.time + fireRate;
            }
        }
    }

    bool PlayerInSight()
    {
        Vector3 toPlayer = playerObj.transform.position - transform.position;

        if (Physics.Raycast(transform.position, toPlayer, out RaycastHit hit, turretRayDistance))
        {
            // If the ray hit the player, the player is in sight
            if (hit.transform == playerObj.transform)
                return true;
        }
        return false;
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = playerObj.transform.position - transform.position;

        // Calculate the rotation needed to look at the player
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Rotate the turret towards the player over time
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    void FireProjectile()
    {
        // Instantiate the projectile at the projectile origin's position and rotation
        projectile = Instantiate(projectileObj, projectileOrigin.position, projectileOrigin.rotation);

        // Set the projectile's velocity to move towards the player
        if (projectile.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = 10f * projectileForce * projectileOrigin.forward;

        // destroy the projectile after some time
        if (projectile != null)
            Destroy(projectile, 2f);
    }
}
