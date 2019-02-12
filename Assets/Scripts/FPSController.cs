using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private Transform firstPersonView, firstPersonCamera;
    private Vector3 FPSRotation = Vector3.zero;

    public float walkSpeed = 6.75f, runSpeed = 10f, crouchSpeed = 4f, jumpSpeed = 8f, gravity = 20f;
    private float speed;

    private bool is_Moving, is_Grounded, is_Crouching;

    private float inputX, inputY;
    private float inputX_Set, inputY_Set;
    private float inputModifyFactor;

    private bool limitDiagonalSpeed = true;

    private float antiBumpFactor = 0.75f;

    private CharacterController charController;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        this.firstPersonView = this.transform.Find("FPS View").transform;
        this.charController = this.GetComponent<CharacterController>();
        this.speed = this.walkSpeed;
        this.is_Moving = false;
    }

    void Update()
    {
        this.playerMovement();
    }

    void playerMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.inputY_Set = 1f;
            }
            else
            {
                this.inputY_Set = -1f;
            }
        }
        else
        {
            this.inputY_Set = 0f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                this.inputX_Set = -1f;
            }
            else
            {
                this.inputX_Set = 1f;
            }
        }
        else
        {
            this.inputX_Set = 0f;
        }

        this.inputY = Mathf.Lerp(this.inputY, this.inputY_Set, Time.deltaTime * 19f);
        this.inputX = Mathf.Lerp(this.inputX, this.inputX_Set, Time.deltaTime * 19f);
        this.inputModifyFactor = Mathf.Lerp(this.inputModifyFactor,
            (this.inputY_Set != 0 && this.inputX_Set != 0 && this.limitDiagonalSpeed) ? 0.75f : 1.0f,
            Time.deltaTime * 19f);

        this.FPSRotation = Vector3.Lerp(FPSRotation, Vector3.zero, Time.deltaTime * 5f);
        this.firstPersonView.localEulerAngles = this.FPSRotation;

        if (this.is_Grounded)
        {
            this.moveDirection = new Vector3(this.inputX * this.inputModifyFactor, -this.antiBumpFactor,
                inputY * inputModifyFactor);

            this.moveDirection = this.transform.TransformDirection(moveDirection) * this.speed;
        }
        this.moveDirection.y -= this.gravity * Time.deltaTime;

        this.is_Grounded = (this.charController.Move(moveDirection * Time.deltaTime) 
            & CollisionFlags.Below) != 0;

        this.is_Moving = this.charController.velocity.magnitude > 0.15f;
    }
}//class
