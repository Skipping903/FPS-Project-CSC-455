using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSPlayerAnimations : NetworkBehaviour
{
    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH = "Crouch";
    private string CROUCH_WALK = "CrouchWalk";
    private string STAND_SHOOT = "StandShoot";
    private string CROUCH_SHOOT = "CrouchShoot";
    private string RELOAD = "Reload";

    public RuntimeAnimatorController animController_Pistol, animController_MachineGun;

    private NetworkAnimator networkAnim;

    void Awake()
    {
        this.anim = this.GetComponent<Animator>();
        this.networkAnim = this.GetComponent<NetworkAnimator>();
    }

    public void Movement(float magnitude)
    {
        this.anim.SetFloat(MOVE, magnitude);
    }

    public void PlayerJump(float velocity)
    {
        this.anim.SetFloat(VELOCITY_Y, velocity);
    }

    public void PlayerCrouch(bool isCrouching)
    {
        this.anim.SetBool(CROUCH, isCrouching);
    }

    public void PlayerCrouchWalk(float magnitude)
    {
        this.anim.SetFloat(CROUCH_WALK, magnitude);
    }

    public void Shoot(bool isStanding)
    {
        if (isStanding)
        {
            this.anim.SetTrigger(STAND_SHOOT);
            this.networkAnim.SetTrigger(STAND_SHOOT);
        }
        else
        {
            this.anim.SetTrigger(CROUCH_SHOOT);
            this.networkAnim.SetTrigger(CROUCH_SHOOT);
        }
    }

    public void ReloadGun()
    {
        this.anim.SetTrigger(RELOAD);
        this.networkAnim.SetTrigger(RELOAD);
    }

    public void ChangeController(bool isPistol)
    {
        if (isPistol)
        {
           this.anim.runtimeAnimatorController = this.animController_Pistol;
        }
        else
        {
           this.anim.runtimeAnimatorController = this.animController_MachineGun;
        }
    }
}//class

