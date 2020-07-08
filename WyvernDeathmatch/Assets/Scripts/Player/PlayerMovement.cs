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

enum MovementSpeed
{
    Running = 0,
    Sprinting = 1,
    Walking = 2     // Might change to crouching if we want to do that
}

public class PlayerMovement : MonoBehaviour
{
    private Vector3 currentVelocity;

    [SerializeField]
    private float speedGround;
    [SerializeField]
    private float sprintSpeedGround;
    [SerializeField]
    private float walkSpeedGround;

    [SerializeField]
    private float speedAir;
    [SerializeField]
    private float sprintSpeedAir;
    [SerializeField]
    private float walkSpeedAir;

    [SerializeField]
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
    private bool pressedCTRLOnce = false;
    private float dropTimer = 0.3f;
    private float dropTimerLeft = 0.0f;

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

    //private AirDodge airDodge;

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

    private PlayerCore playerCore;

    #region Raycast Origins
    private Vector3 front, back, right, left;
    #endregion

    [SerializeField]
    private float maxDescentSlopeAngle = 80.0f;

    LayerMask landableMask;

    private Vector3 lastCollisionPoint;

    // Start is called before the first frame update
    void Start()
    { 
        playerState = PlayerState.InAir;
        characterController = GetComponent<CharacterController>();
        rotationAmount = delta;
        playerCore = GetComponent<PlayerCore>();
        landableMask = LayerMask.GetMask("LandableSurface");

        //airDodge = GetComponent<AirDodge>();

        // Temp
        currentTurningSpeed = turningSpeedAir;
        currentSpeed = speedAir;
        currentSpeedVertical = speedVerticalAir;
        currentGravity = 0.0f;
        currentVerticalMovement = Vector3.zero;

        lastCollisionPoint = Vector3.zero;

        // Make a camera for the player
    }

    // Update is called once per frame
    void Update()
    {
        if (pressedCTRLOnce)
        {
            dropTimerLeft -= Time.deltaTime;

            if (dropTimerLeft <= delta)
            {
                pressedCTRLOnce = false;
            }
        }
        // Change transition to falling/onGround to factor in whether or not the player is grounded
        // Debug.Log(characterController.isGrounded);
    }

    // Note for later: Maybe only use physics-based movement for aerial movement
    // ToDo: Change direction to move toward crosshair. Right now, it's hard to tell where the character is going. May need to use some arbitrary point.
    private void MovePlayer()
    {
        Sprint();

        playerCamera.LastPlayerPosition = transform.position;

        switch (playerState)
        {
            case PlayerState.InAir:
                if(playerCore.FlightStamina < delta)
                {
                    playerState = PlayerState.Falling;
                    currentGravity = gravity;
                }
                else
                {
                    playerCore.FlightStamina -= playerCore.FlightStaminaConsumptionRate;
                }
                velocity.y = 0.0f;
                playerCore.Stamina += playerCore.StaminaReplenishRate;
                break;
            case PlayerState.OnGround:
                velocity.y = 0.0f;
                playerCore.FlightStamina += playerCore.FlightStaminaReplenishRate;
                break;
        }
        
        /*if(playerState == PlayerState.InAir || playerState == PlayerState.OnGround)
        {
            velocity.y = 0.0f;
        }*/

        velocity = new Vector3(0.0f, velocity.y, 0.0f);
        velocity += transform.forward * Input.GetAxis("Vertical") * currentSpeed;
        velocity += transform.right * Input.GetAxis("Horizontal") * currentSpeed;

        // Descending slope
        if(playerState == PlayerState.OnGround && !Input.GetKey(KeyCode.Space))
        {
            UpdateOrigins();
            velocity.y = DescendSlope().y - transform.position.y;
        }

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
            if (!pressedCTRLOnce)
            {
                if (Input.GetKeyDown(KeyCode.LeftControl)){
                    pressedCTRLOnce = true;
                    dropTimerLeft = dropTimer;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    pressedCTRLOnce = false;
                    playerState = PlayerState.Falling;
                    currentGravity = gravity;
                }
            }
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
        if (hit.collider.gameObject.layer == 9 && characterController.isGrounded)
        {
            if (playerState != PlayerState.OnGround)
            {
                playerState = PlayerState.OnGround;
                currentSpeed = speedGround;
                currentSpeedVertical = speedVerticalGround;
                currentTurningSpeed = turningSpeedGround;
                currentGravity = gravity;
                lastCollisionPoint = hit.point;
            }
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            switch (playerState)
            {
                case PlayerState.OnGround:
                    if (playerCore.Stamina >= delta)
                    {
                        currentSpeed = sprintSpeedGround;
                    }
                    break;
                case PlayerState.InAir:
                    if (playerCore.FlightStamina >= delta)
                    {
                        currentSpeed = sprintSpeedAir;
                    }
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift) && characterController.velocity.magnitude >= delta)
        {
            switch (playerState)
            {
                case PlayerState.OnGround:
                    if (playerCore.Stamina >= delta)
                    {
                        playerCore.Stamina -= playerCore.StaminaConsumptionRate;
                    }
                    break;
                case PlayerState.InAir:
                    if (playerCore.FlightStamina >= delta)
                    {
                        playerCore.FlightStamina -= playerCore.FlightStaminaConsumptionRate;
                    }
                    break;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            switch (playerState)
            {
                case PlayerState.OnGround:
                    currentSpeed = speedGround;
                    break;
                case PlayerState.InAir:
                    currentSpeed = speedAir;
                    break;
            }
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            switch (playerState)
            {
                case PlayerState.OnGround:
                    playerCore.FlightStamina += playerCore.FlightStaminaReplenishRate;
                    playerCore.Stamina += playerCore.StaminaReplenishRate;
                    break;
                default:
                    playerCore.Stamina += playerCore.StaminaReplenishRate;
                    break;
            }
        }
    }

    private void UpdateOrigins()
    {
        // Need to find out how to get right and left from velocity
        front = transform.position + (velocity.normalized * (characterController.radius - characterController.skinWidth));
        back = transform.position + (-velocity.normalized * (characterController.radius - characterController.skinWidth));
        right = transform.position + ((Quaternion.AngleAxis(90.0f, transform.up) * new Vector3(velocity.x, 0.0f, velocity.z).normalized) * (characterController.radius - characterController.skinWidth));
        left = transform.position + (-(Quaternion.AngleAxis(90.0f, transform.up) * new Vector3(velocity.x, 0.0f, velocity.z).normalized) * (characterController.radius - characterController.skinWidth));
    }

    private Vector3 DescendSlope()
    {
        float directionZ = Mathf.Sign(velocity.z);
        float directionX = Mathf.Sign(velocity.x);

        RaycastHit hit = new RaycastHit();
        Vector3 output = Vector3.zero;

        // Need to find angle and see if descending
        /*if(directionZ >= 0.0f)
        {
            RaycastHit hitFront = RaycastToGround(front);
            if (directionX > 0.0f)
            {
                RaycastHit hitRight = RaycastToGround(right);
                hit = hitFront.point.y > hitRight.point.y ? hitFront : hitRight;
            }
            else if(directionX < 0.0f)
            {
                RaycastHit hitLeft = RaycastToGround(left);
                hit = hitFront.point.y > hitLeft.point.y ? hitFront : hitLeft;
            }
        }
        else if (directionZ < 0.0f)
        {
            RaycastHit hitBack = RaycastToGround(back);
            if (directionX > 0.0f)
            {
                RaycastHit hitRight = RaycastToGround(right);
                hit = hitBack.point.y > hitRight.point.y ? hitBack : hitRight;
            }
            else if (directionX < 0.0f)
            {
                RaycastHit hitLeft = RaycastToGround(left);
                hit = hitBack.point.y > hitLeft.point.y ? hitBack : hitLeft;
            }
        }*/

        Vector3 pointOnCollider = transform.position + velocity + (lastCollisionPoint - transform.position);
        hit = RaycastToGround(pointOnCollider);

        if (hit.distance > 0.0f)
        {
            float angle = Vector3.Angle(hit.normal, transform.up);
            if(angle != 0.0f && angle <= maxDescentSlopeAngle)
            {
                if(Mathf.Sign(hit.normal.z) == velocity.z && Mathf.Sign(hit.normal.x) == velocity.x)
                {
                    output.y = hit.point.y;
                }
            }
        }
        return Vector3.zero;
    }

    private RaycastHit RaycastToGround(Vector3 origin)
    {
        RaycastHit hit;
        Physics.Raycast(origin, -transform.up, out hit, Mathf.Infinity, landableMask);
        return hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(90.0f, transform.up) * new Vector3(velocity.x, 0.0f, velocity.z).normalized * 5.0f); //Vector3.Cross(velocity, Vector3.up) * 5.0f);
    }
}
