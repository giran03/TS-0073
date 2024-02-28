using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Transform laserOrigin;
    [SerializeField] Transform laserLookAt;
    [SerializeField] GameObject[] laserActivatedObjects;    // flips the active state of the game objects in this array

    LineRenderer lr;
    RaycastHit laserHit;
    bool isEnabled;
    private void Start() => lr = GetComponent<LineRenderer>();

    private void Update()
    {
        lr.SetPosition(0, laserOrigin.position);
        lr.SetPosition(1, laserLookAt.position);

        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out laserHit))
        {
            if (laserHit.collider != null)
            {
                lr.SetPosition(1, laserHit.point);

                if (laserHit.collider.gameObject.CompareTag("LaserBlockObj"))
                    EnableObject(true);
                else
                    EnableObject(false);
            }
            else
                lr.SetPosition(1, transform.forward * 100f);
        }
    }

    void EnableObject(bool enable)
    {
        if (isEnabled != enable)
        {
            foreach (GameObject obj in laserActivatedObjects)
                obj.SetActive(!obj.activeSelf);
            isEnabled = enable;
        }
    }
}
