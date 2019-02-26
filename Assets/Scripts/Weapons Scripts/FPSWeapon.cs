using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    private GameObject muzzleFlash;

    void Awake()
    {
        this.muzzleFlash = this.transform.Find("Muzzle Flash").gameObject;
        this.muzzleFlash.SetActive(false);
    }

    public void Shoot()
    {
        StartCoroutine(this.TurnOnMuzzleFlash());
    }

    IEnumerator TurnOnMuzzleFlash()
    {
        this.muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        this.muzzleFlash.SetActive(false);
    }
}//class
