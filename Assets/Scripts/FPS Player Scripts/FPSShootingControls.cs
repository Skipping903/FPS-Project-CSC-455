using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShootingControls : MonoBehaviour
{
    private Camera mainCam;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private GameObject concrete_Impact;

    public float damageAmount = 5f;

    void Start()
    {
        this.mainCam = this.transform.Find("FPS View").Find("FPS Camera").GetComponent<Camera>();
    }

    void Update()
    {
        this.Shoot();
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > this.nextTimeToFire)
        {
            this.nextTimeToFire = Time.time + 1f / this.fireRate;

            RaycastHit hit;

            if (Physics.Raycast(mainCam.transform.position, this.mainCam.transform.forward, out hit))
            {
                Instantiate(this.concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
} // class