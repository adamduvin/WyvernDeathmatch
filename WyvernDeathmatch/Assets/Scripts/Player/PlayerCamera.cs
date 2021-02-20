using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 currentOffset;

    public Vector3 CurrentOffset
    {
        get { return currentOffset; }
    }

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector3 offsetFlight;

    [SerializeField]
    private Vector3 offsetADS;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoomSmoothing = 0.5f;

    [SerializeField]
    [Range(0.0f, 179)]
    private float defaultFOV = 60;

    [SerializeField]
    [Range(0.0f, 179)]
    private float zoomFOV = 20;

    private CameraState cameraState;

    private PlayerCameraParent playerCameraParent;

    private Transform parentTransform;

    private PlayerMovement playerMovement;

    private Vector3 velocity = Vector3.zero;

    private Camera followCamera;

    [SerializeField]
    private GameObject lookAtTarget;

    private LayerMask maskExcludeSoftBoundary;

    [SerializeField]
    private GameObject originalSpot;

    // Start is called before the first frame update
    void Start()
    {
        playerCameraParent = GetComponentInParent<PlayerCameraParent>();
        parentTransform = playerCameraParent.transform;
        cameraState = CameraState.Normal;
        followCamera = GetComponent<Camera>();
        maskExcludeSoftBoundary = ~LayerMask.GetMask("SoftBoundary");
        currentOffset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            AimDownSights(offsetADS, zoomFOV);
            cameraState = CameraState.AimDownSights;
        }
        else if (!Input.GetMouseButton(1))
        {
            if(playerMovement.PlayerMovementState == PlayerState.InAir)
            {
                AimDownSights(offsetFlight, defaultFOV);
            }
            else
            {
                AimDownSights(offset, defaultFOV);
            }
            
            cameraState = CameraState.Normal;
        }

        playerCameraParent.RotateCameraParent();
        playerMovement.RotatePlayer(Convert.ToBoolean(cameraState), playerCameraParent.transform.rotation, playerCameraParent.RotationSmoothing);
        playerMovement.MovePlayer();
        playerCameraParent.MoveCameraParent();
        MoveCamera();
    }

    // Moves the camera and looks at the lookAtTarget
    public void MoveCamera()
    {
        Vector3 posWithOffset = Vector3.zero;   // Parent's origin
        posWithOffset += transform.InverseTransformDirection(parentTransform.forward) * currentOffset.z;
        posWithOffset += transform.InverseTransformDirection(parentTransform.right) * currentOffset.x;
        posWithOffset += transform.InverseTransformDirection(parentTransform.up) * currentOffset.y;
        transform.localPosition = posWithOffset;

        /*RaycastHit posHit;

        Vector3 origin = playerCameraParent.transform.position + (transform.InverseTransformDirection(transform.up) * currentOffset.y);
        Vector3 target = playerCameraParent.transform.position + (transform.rotation * currentOffset);

        Debug.Log(target);

        if (Physics.Raycast(origin, target, out posHit, (origin - target).magnitude, maskExcludeSoftBoundary))
        {
            //Debug.Log(currentOffset.z);
            transform.position = posHit.point;
        }*/

        RaycastHit hit;
        Vector3 hitLocation;
        if (Physics.Raycast(transform.position, lookAtTarget.transform.position - originalSpot.transform.position, out hit, Mathf.Infinity, maskExcludeSoftBoundary))
        {
            hitLocation = hit.point;
        }
        else
        {
            hitLocation = lookAtTarget.transform.position;
        }

        //transform.LookAt(hitLocation);
    }

    // Softly moves between aiming modes
    private void AimDownSights(Vector3 target, float FOV)
    {
        currentOffset = Vector3.SmoothDamp(currentOffset, target, ref velocity, zoomSmoothing, 10f);

        followCamera.fieldOfView = Mathf.Lerp(followCamera.fieldOfView, FOV, zoomSmoothing);
    }

    // Camera initialization
    public void SetUpCamera(PlayerMovement playerMovement, GameObject lookAtTarget)
    {
        this.playerMovement = playerMovement;
        this.lookAtTarget = lookAtTarget;
        originalSpot.transform.position += offset;
    }
}
