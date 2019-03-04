using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHandsWeapon : MonoBehaviour
{
    public AudioClip shootClip, reloadClip;
    private AudioSource audioManager;
    private GameObject muzzleFlash;

    private Animator anim;

    private string SHOOT = "Shoot";
    private string RELOAD = "Reload";

    void Awake()
    {
        this.muzzleFlash = this.transform.Find("MuzzleFlash").gameObject;
        this.muzzleFlash.SetActive(false);

        this.audioManager = this.GetComponent<AudioSource>();
        this.anim = this.GetComponent<Animator>();
    }

    public void Shoot()
    {
        if (this.audioManager.clip != this.shootClip)
        {
            this.audioManager.clip = this.shootClip;
        }
        this.audioManager.Play();

        StartCoroutine(this.TurnMuzzleFlashOn());

        this.anim.SetTrigger(SHOOT);
    }

    IEnumerator TurnMuzzleFlashOn()
    {
        this.muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        this.muzzleFlash.SetActive(false);
    }

    public void Reload()
    {
        StartCoroutine(this.PlayReloadSound());
        this.anim.SetTrigger(RELOAD);
    }

    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        if (this.audioManager.clip != this.reloadClip)
        {
            this.audioManager.clip = this.reloadClip;
        }
        this.audioManager.Play();
    }
} // class