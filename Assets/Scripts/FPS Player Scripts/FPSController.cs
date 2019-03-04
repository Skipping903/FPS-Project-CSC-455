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

    public LayerMask groundLayer;
    private float rayDistance;
    private float default_ControllerHeight;
    private Vector3 default_CamPos;
    private float camHeight;

    private FPSPlayerAnimations playerAnimation;

    [SerializeField] private WeaponManager weapon_Manager;
    private FPSWeapon current_Weapon;
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private WeaponManager handsWeapon_Manager;
    private FPSHandsWeapon current_Hands_Weapon;

    void Start()
    {
        this.firstPersonView = this.transform.Find("FPS View").transform;
        this.charController = this.GetComponent<CharacterController>();
        this.speed = this.walkSpeed;
        this.is_Moving = false;

        this.rayDistance = this.charController.height * 0.5f + this.charController.radius;
        this.default_ControllerHeight = this.charController.height;
        this.default_CamPos = this.firstPersonView.localPosition;

        this.playerAnimation = this.GetComponent<FPSPlayerAnimations>();

        this.weapon_Manager.weapons[0].SetActive(true);
        this.current_Weapon = this.weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

        this.handsWeapon_Manager.weapons[0].SetActive(true);
        this.current_Hands_Weapon = this.handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
    }

    void Update()
    {
        this.playerMovement();
        this.SelectWeapon();
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
            this.PlayerCrouchingAndSprinting();

            this.moveDirection = new Vector3(this.inputX * this.inputModifyFactor, -this.antiBumpFactor,
                inputY * inputModifyFactor);

            this.moveDirection = this.transform.TransformDirection(moveDirection) * this.speed;

            this.PlayerJump();
        }
        this.moveDirection.y -= this.gravity * Time.deltaTime;

        this.is_Grounded = (this.charController.Move(moveDirection * Time.deltaTime) 
            & CollisionFlags.Below) != 0;

        this.is_Moving = this.charController.velocity.magnitude > 0.15f;

        this.HandleAnimations();
    }

    void PlayerCrouchingAndSprinting()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!is_Crouching)
            {
                this.is_Crouching = true;
            }
            else
            {
                if (CanGetUp())
                {
                    this.is_Crouching = false;
                }
            }
            this.StopCoroutine(MoveCameraCrouch());
            this.StartCoroutine(MoveCameraCrouch());
        }
        if (this.is_Crouching)
        {
            this.speed = this.crouchSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                this.speed = this.runSpeed;
            }
            else
            {
                this.speed = this.walkSpeed;
            }
        }
        this.playerAnimation.PlayerCrouch(is_Crouching);
    }

    bool CanGetUp()
    {
        Ray groundRay = new Ray(transform.position, transform.up);
        RaycastHit groundHit;

        if (Physics.SphereCast(groundRay, charController.radius + 0.05f,
            out groundHit, rayDistance, groundLayer))
        {

            if (Vector3.Distance(transform.position, groundHit.point) < 2.3f)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator MoveCameraCrouch()
    {
        this.charController.height = is_Crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
        this.charController.center = new Vector3(0f, charController.height / 2f, 0f);

        camHeight = is_Crouching ? default_CamPos.y / 1.5f : default_CamPos.y;

        while (Mathf.Abs(camHeight - firstPersonView.localPosition.y) > 0.01f)
        {

            this.firstPersonView.localPosition = Vector3.Lerp(firstPersonView.localPosition,
                new Vector3(this.default_CamPos.x, this.camHeight, this.default_CamPos.z),
                Time.deltaTime * 11f);

            yield return null;
        }
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (is_Crouching)
            {

                if (CanGetUp())
                {
                    this.is_Crouching = false;

                     this.playerAnimation.PlayerCrouch(is_Crouching);

                    this.StopCoroutine(MoveCameraCrouch());
                    this.StartCoroutine(MoveCameraCrouch());
                }

            }
            else
            {
                this.moveDirection.y = this.jumpSpeed;
            }
        }
    }

    void HandleAnimations()
    {
        this.playerAnimation.Movement(this.charController.velocity.magnitude);
        this.playerAnimation.PlayerJump(this.charController.velocity.y);

        if (this.is_Crouching && this.charController.velocity.magnitude > 0f)
        {
            this.playerAnimation.PlayerCrouchWalk(this.charController.velocity.magnitude);
        }

        if (Input.GetMouseButtonDown(0) && Time.time > this.nextTimeToFire)
        {
            this.nextTimeToFire = Time.time + 1f / this.fireRate;

            if (this.is_Crouching)
            {
                this.playerAnimation.Shoot(false);
            }
            else
            {
                this.playerAnimation.Shoot(true);
            }
            this.current_Weapon.Shoot();
            this.current_Hands_Weapon.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.playerAnimation.ReloadGun();
            this.current_Hands_Weapon.Reload();
        }
    }

    void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!this.handsWeapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < this.handsWeapon_Manager.weapons.Length; i++)
                {
                    this.handsWeapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Hands_Weapon = null;

                this.handsWeapon_Manager.weapons[0].SetActive(true);
                this.current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
            }

            if (!this.weapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < this.weapon_Manager.weapons.Length; i++)
                {
                    this.weapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Weapon = null;
                this.weapon_Manager.weapons[0].SetActive(true);
                this.current_Weapon = this.weapon_Manager.weapons[0].GetComponent<FPSWeapon>();
                this.playerAnimation.ChangeController(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!this.handsWeapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < this.handsWeapon_Manager.weapons.Length; i++)
                {
                    this.handsWeapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Hands_Weapon = null;

                this.handsWeapon_Manager.weapons[1].SetActive(true);
                this.current_Hands_Weapon = this.handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
            }

            if (!this.weapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < this.weapon_Manager.weapons.Length; i++)
                {
                    this.weapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Weapon = null;
                this.weapon_Manager.weapons[1].SetActive(true);
                this.current_Weapon = this.weapon_Manager.weapons[1].GetComponent<FPSWeapon>();
                this.playerAnimation.ChangeController(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!this.handsWeapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < this.handsWeapon_Manager.weapons.Length; i++)
                {
                    this.handsWeapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Hands_Weapon = null;

                this.handsWeapon_Manager.weapons[2].SetActive(true);
                this.current_Hands_Weapon = this.handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
            }

            if (!this.weapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < this.weapon_Manager.weapons.Length; i++)
                {
                    this.weapon_Manager.weapons[i].SetActive(false);
                }

                this.current_Weapon = null;
                this.weapon_Manager.weapons[2].SetActive(true);
                this.current_Weapon = this.weapon_Manager.weapons[2].GetComponent<FPSWeapon>();
                this.playerAnimation.ChangeController(false);
            }
        }
    }
}//class
