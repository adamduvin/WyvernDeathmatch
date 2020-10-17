using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

enum CameraState
{
    Normal,
    AimDownSights,
    IntroOutro
}

public class PlayerCameraParent : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float movementSmoothing = 0.5f;
    [Range(0.0f, 1.0f)]
    private float rotationSmoothing = 0.5f;

    public float RotationSmoothing
    {
        get { return rotationSmoothing; }
    }

    private float currentTurningSpeed;
    [SerializeField]
    private float turningSpeedAir;
    [SerializeField]
    private float turningSpeedGround;

    private Quaternion originalRotation;

    [SerializeField]
    private float maxDistance = 5.0f;
    [SerializeField]
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        currentTurningSpeed = turningSpeedAir;
        originalRotation = transform.rotation;
    }

    // Rotates the camera's parent object
    public void RotateCameraParent()
    {
        Quaternion rotationTarget = Quaternion.identity;

        transform.rotation = originalRotation;

        if (Mathf.Abs(Input.GetAxis("Mouse X")) >= delta)
        {
            rotationTarget *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * currentTurningSpeed * Time.deltaTime, Vector3.up);
        }

        if (Mathf.Abs(Input.GetAxis("Mouse Y")) >= delta)
        {
            float angle = -Input.GetAxis("Mouse Y") * currentTurningSpeed * Time.deltaTime;

            if (transform.rotation.eulerAngles.x + angle > 90.0f && transform.rotation.eulerAngles.x + angle < 180.0f)
            {
                angle = 89.9f - transform.rotation.eulerAngles.x;
            }
            else if (transform.rotation.eulerAngles.x + angle < 270.0f && transform.rotation.eulerAngles.x + angle > 180.0f)
            {
                angle = 270.1f - transform.rotation.eulerAngles.x;
            }

            rotationTarget *= Quaternion.AngleAxis(angle, transform.right);
        }
        transform.rotation = rotationTarget * transform.rotation;

        originalRotation = transform.rotation;
    }

    // Moves the camera parent towards the player's position
    public void MoveCameraParent()
    {
        transform.position = Vector3.SmoothDamp(transform.position, playerTransform.position, ref velocity, movementSmoothing);
    }

    // Initialize the player camera
    public void SetUpCamera(PlayerMovement playerMovement, GameObject lookAtTarget, Transform playerTransform)
    {
        this.playerTransform = playerTransform;

        GetComponentInChildren<PlayerCamera>().SetUpCamera(playerMovement, lookAtTarget);
    }
}
