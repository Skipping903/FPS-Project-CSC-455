using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShootingControls : NetworkBehaviour
{
    private Camera mainCam;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private GameObject concrete_Impact, blood_Impact;

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
                if (hit.transform.tag == "Enemy")
                {
                    this.CmdDealDamage(hit.transform.gameObject, hit.point, hit.normal);
                }
                else
                {
                    Instantiate(this.concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }

    [Command]
    void CmdDealDamage(GameObject obj, Vector3 pos, Vector3 rotation)
    {
        obj.GetComponent<PlayerHealth>().TakeDamage(this.damageAmount);

        Instantiate(this.blood_Impact, pos, Quaternion.LookRotation(rotation));

    }
} // class