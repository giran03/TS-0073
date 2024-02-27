using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Transform laserOrigin;
    [SerializeField] Transform laserLookAt;
    [SerializeField] GameObject[] gameObjectsArray;

    LineRenderer lr;
    RaycastHit hit;
    bool isEnabled;
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        lr.SetPosition(0, laserOrigin.position);
        lr.SetPosition(1, laserLookAt.position);

        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit))
        {
            if (hit.collider != null)
            {
                lr.SetPosition(1, hit.point);

                if (hit.collider.gameObject.CompareTag("Draggable"))
                    EnableObject(true);
                else
                    EnableObject(false);
            }
            else
                lr.SetPosition(1, transform.forward * 100f);
        }
        // else
        //     lr.SetPosition(1, transform.forward * 100f);
    }

    void EnableObject(bool enable)
    {
        if (isEnabled != enable)
        {
            foreach (GameObject obj in gameObjectsArray)
                obj.SetActive(!obj.activeSelf);
            isEnabled = enable;
        }
    }
}
