using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

enum PlayerState
{
    OnGround = 0,
    InAir = 1,
    Landing = 2,    // Animation
    TakingOff = 3,  // Animation
    Jumping = 4,
    Falling = 5
}

public class PlayerMovement : MonoBehaviour
{
    private Vector3 currentVelocity;

    [SerializeField]
    private float speedGround;

    [SerializeField]
    private float speedAir;

    private float currentSpeed;

    private float currentMaxSpeed;
    private float currentMaxForce;

    [SerializeField]
    private float speedVerticalGround;

    [SerializeField]
    private float speedVerticalAir;

    private float currentSpeedVertical;

    private int jumpModifier = 0;

    private Vector3 currentVerticalMovement;

    [SerializeField]
    private float turningSpeedGround;

    public float TurningSpeedGround
    {
        get { return turningSpeedGround; }
    }

    [SerializeField]
    private float turningSpeedAir;

    public float TurningSpeedAir
    {
        get { return turningSpeedAir; }
    }

    private float currentTurningSpeed;

    [SerializeField]
    private Vector3 velocity;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rotationSmoothing = 0.5f;

    private PlayerState playerState;

    //private Rigidbody rb;
    private CharacterController characterController;

    [SerializeField]
    private PlayerCamera playerCamera;        // Instantiate this in start with a prefab

    [SerializeField]
    private bool isRotatingWithCamera = true;
    public bool IsRotatingWithCamera
    {
        get { return isRotatingWithCamera; }
    }

    private float rotationAmount;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float groundGravity;

    private float currentGravity;

    // Start is called before the first frame update
    void Start()
    { 
        playerState = PlayerState.InAir;
        characterController = GetComponent<CharacterController>();
        rotationAmount = delta;

        // Temp
        currentTurningSpeed = turningSpeedAir;
        currentSpeed = speedAir;
        currentSpeedVertical = speedVerticalAir;
        currentGravity = 0.0f;
        currentVerticalMovement = Vector3.zero;

        // Make a camera for the player
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    // Note for later: Maybe only use physics-based movement for aerial movement
    // ToDo: Change direction to move toward crosshair. Right now, it's hard to tell where the character is going. May need to use some arbitrary point.
    private void MovePlayer()
    {
        playerCamera.LastPlayerPosition = transform.position;
        
        if(playerState == PlayerState.InAir || playerState == PlayerState.OnGround)
        {
            velocity.y = 0.0f;
        }

        velocity = new Vector3(0.0f, velocity.y, 0.0f);
        velocity += transform.forward * Input.GetAxis("Vertical") * currentSpeed;
        velocity += transform.right * Input.GetAxis("Horizontal") * currentSpeed;

        Jump();
        velocity += Vector3.up * -currentGravity;

        characterController.Move(velocity * Time.deltaTime);
    }

    public void RotatePlayer(bool isADS)
    {
        if(isRotatingWithCamera || isADS || characterController.velocity.magnitude >= delta)
        {
            Quaternion rotationTarget = playerCamera.transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, playerCamera.rotationSmoothing);
            if(Quaternion.Angle(transform.rotation, rotationTarget) < delta)
            {
                transform.rotation = rotationTarget;
            }
            if (playerState == PlayerState.OnGround || playerState == PlayerState.Falling || playerState == PlayerState.Jumping)
            {
                Vector3 tempRotation = transform.rotation.eulerAngles;
                tempRotation.x = 0.0f;
                tempRotation.z = 0.0f;
                transform.rotation = Quaternion.Euler(tempRotation);
            }
        }
        MovePlayer();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity += Vector3.up * Input.GetAxis("Jump") * currentSpeedVertical;

            switch (playerState)
            {
                case PlayerState.OnGround:
                    playerState = PlayerState.Jumping;
                    currentSpeedVertical = 0.0f;
                    break;
                case PlayerState.Falling:
                case PlayerState.Jumping:
                    playerState = PlayerState.InAir;
                    currentSpeed = speedAir;
                    currentSpeedVertical = speedVerticalAir;
                    currentTurningSpeed = turningSpeedAir;
                    currentGravity = 0.0f;
                    currentSpeedVertical = speedVerticalAir;
                    break;
                default:
                    break;
            }
        }
        else if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl)) && playerState == PlayerState.InAir)
        {
            velocity += Vector3.up * Input.GetAxis("Jump") * currentSpeedVertical;
        }
        if(playerState == PlayerState.OnGround)
        {
            if (!characterController.isGrounded)
            {
                playerState = PlayerState.Falling;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Need to update once animations are added to handle taking off and landing
        if (hit.collider.gameObject.layer == 9)
        {
            if (playerState != PlayerState.OnGround)
            {
                playerState = PlayerState.OnGround;
                currentSpeed = speedGround;
                currentSpeedVertical = speedVerticalGround;
                currentTurningSpeed = turningSpeedGround;
                currentGravity = gravity;
            }
        }
    }
}
