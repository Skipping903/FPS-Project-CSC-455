using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseX, MouseY }
    public RotationAxes axes = RotationAxes.MouseY;

    private float currentSensivity_X = 1.5f;
    private float currentSensivity_Y = 1.5f;

    private float sensivity_X = 1.5f;
    private float sensivity_Y = 1.5f;

    private float rotation_X, rotation_Y;

    private float minimum_X = -360f;
    private float maximum_X = 360f;

    private float minimum_Y = -60f;
    private float maximum_Y = 60f;

    private Quaternion originalRotation;

    private float mouseSensivity = 1.7f;

    void Start()
    {
        this.originalRotation = this.transform.rotation;
    }

    private void LateUpdate()
    {
        this.HandleRotation();
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }

        if (angle > 360f)
        {
            angle -= 360f;
        }
        return Mathf.Clamp(angle, min, max);
    }

    void HandleRotation()
    {
        if (this.currentSensivity_X != this.mouseSensivity || this.currentSensivity_Y != this.mouseSensivity)
        {
            this.currentSensivity_X = this.currentSensivity_Y = this.mouseSensivity;
        }

        this.sensivity_X = this.currentSensivity_X;
        this.sensivity_Y = this.currentSensivity_Y;

        if (this.axes == RotationAxes.MouseX)
        {
            this.rotation_X += Input.GetAxis("Mouse X") * this.sensivity_X;

            this.rotation_X = this.ClampAngle(this.rotation_X, this.minimum_X, this.maximum_X);
            Quaternion xQuaternion = Quaternion.AngleAxis(this.rotation_X, Vector3.up);

            this.transform.localRotation = this.originalRotation * xQuaternion;
        }

        if (this.axes == RotationAxes.MouseY)
        {
            this.rotation_Y += Input.GetAxis("Mouse Y") * this.sensivity_Y;

            this.rotation_Y = this.ClampAngle(this.rotation_Y, this.minimum_Y, this.maximum_Y);

            Quaternion yQuaternion = Quaternion.AngleAxis(-rotation_Y, Vector3.right);

            this.transform.localRotation = this.originalRotation * yQuaternion;
        }
    }
}//class
