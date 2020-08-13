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

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float movementSmoothing = 0.5f;
    [Range(0.0f, 1.0f)]
    public float rotationSmoothing = 0.5f;

    private Vector3 currentOffset;

    public Vector3 CurrentOffset
    {
        get { return currentOffset; }
    }

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector3 offsetADS;

    [SerializeField]
    private float offsetZ;
    [SerializeField]
    private float offsetZADS;
    private float currentOffsetZ;

    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float offsetXADS;
    private float currentOffsetX;

    [SerializeField]
    private float offsetY;
    [SerializeField]
    private float offsetYADS;
    private float currentOffsetY;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoomSmoothing = 0.5f;

    [SerializeField]
    [Range(0.0f, 179)]
    private float defaultFOV = 60;

    [SerializeField]
    [Range(0.0f, 179)]
    private float zoomFOV = 20;

    private float currentFOV;

    private float currentTurningSpeed;
    private float turningSpeedAir;
    private float turningSpeedGround;

    private Vector3 movementTarget;

    private CameraState cameraState;

    private CharacterController playerController;

    private Vector3 lastPlayerPosition;

    public Vector3 LastPlayerPosition
    {
        set { lastPlayerPosition = value; }
    }

    [SerializeField]
    private Vector3 lookAtTarget;

    private Quaternion originalRotation;

    private LayerMask maskExcludeProjectiles;

    [SerializeField]
    private float maxDistance = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Might want to start setting everything here
        lastPlayerPosition = playerMovement.gameObject.transform.position;
        playerController = playerMovement.GetComponent<CharacterController>();
        maskExcludeProjectiles =~ LayerMask.GetMask("Projectile");
    }

    private void OnEnable()
    {
        currentOffset = offset;

        playerMovement.GetComponent<PlayerShoot>().Distance = currentOffset.z;

        turningSpeedAir = playerMovement.TurningSpeedAir;
        turningSpeedGround = playerMovement.TurningSpeedGround;
        currentTurningSpeed = turningSpeedAir;
        movementTarget = playerMovement.transform.position;
        cameraState = CameraState.Normal;   // Temp
        currentFOV = defaultFOV;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            AimDownSights(offsetADS, zoomFOV);
            cameraState = CameraState.AimDownSights;
        }
        if (!Input.GetMouseButton(1))
        {
            AimDownSights(offset, defaultFOV);
            cameraState = CameraState.Normal;
        }
        RotateCamera();
    }

    private void RotateCamera()
    {
        Quaternion rotationTarget = Quaternion.identity;

        // Have camera look at a raycast hit
        transform.rotation = originalRotation;

        if (Mathf.Abs(Input.GetAxis("Mouse X")) >= delta)
        {
            rotationTarget *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * currentTurningSpeed * Time.deltaTime, Vector3.up);
        }

        // Might need to fix. Seems there is z rotation. Should see if it matters later
        if (Mathf.Abs(Input.GetAxis("Mouse Y")) >= delta)
        {
            float angle = -Input.GetAxis("Mouse Y") * currentTurningSpeed * Time.deltaTime;

            //Debug.Log(transform.rotation.eulerAngles.x);
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
        
        /*if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            //Debug.Log("Hit: " + hit.point);
            lookAtTarget = hit.point;
        }
        else
        {
            Debug.Log("Missed: " + hit.point);
            lookAtTarget = transform.position + transform.forward * 20.0f;
        }*/

        playerMovement.RotatePlayer(Convert.ToBoolean(cameraState));
        transform.position = lastPlayerPosition;
        MoveCamera();   // Set movement smoothing here as well as rotation smoothing
    }

    // Handle OnPlayerMove event here
    public void MoveCamera()
    {
        movementTarget = playerTransform.transform.position;
        GetComponent<Camera>().fieldOfView = currentFOV;
        Vector3 targetToCamera = movementTarget - transform.position;
        /*if (Vector3.Magnitude(targetToCamera) > maxDistance)
        {
            transform.position = movementTarget - (targetToCamera.normalized * maxDistance);
        }*/

        transform.position = Vector3.Lerp(transform.position, movementTarget, movementSmoothing);
        if (Vector3.Magnitude(targetToCamera) <= delta)
        {
            transform.position = movementTarget;
        }

        Vector3 posWithOffset = transform.position;
        posWithOffset -= transform.forward * offset.z;
        posWithOffset += transform.right * offset.x;
        posWithOffset += transform.up * offset.y;
        RaycastHit hit;
        if (Physics.Raycast(posWithOffset, originalRotation * Vector3.forward, out hit, Mathf.Infinity, maskExcludeProjectiles))
        {
            lookAtTarget = hit.point;
        }
        else
        {
            lookAtTarget = posWithOffset + (originalRotation * Vector3.forward * 1000.0f);
        }

        transform.position -= transform.forward * currentOffset.z;
        transform.position += transform.right * currentOffset.x;
        transform.position += transform.up * currentOffset.y;
        //Debug.Log("Target: " + lookAtTarget);
        transform.LookAt(lookAtTarget);
    }

    private void AimDownSights(Vector3 target, float FOV)
    {
        Vector3 prevOffset = currentOffset;
        currentOffset.x = Mathf.Lerp(currentOffset.x, target.x, zoomSmoothing);
        currentOffset.y = Mathf.Lerp(currentOffset.y, target.y, zoomSmoothing);
        currentOffset.z = Mathf.Lerp(currentOffset.z, target.z, zoomSmoothing);
        playerMovement.GetComponent<PlayerShoot>().Distance = currentOffset.z;

        currentFOV = Mathf.Lerp(currentFOV, FOV, zoomSmoothing);
    }
}
