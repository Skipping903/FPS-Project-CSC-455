using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerAnimations : MonoBehaviour
{
    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH = "Crouch";
    private string CROUCH_WALK = "CrouchWalk";

    void Awake()
    {
        this.anim = this.GetComponent<Animator>();
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
}//class

