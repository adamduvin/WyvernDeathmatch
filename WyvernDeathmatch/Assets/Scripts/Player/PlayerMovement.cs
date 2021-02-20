using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using static Globals;

public enum PlayerState
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
    Walking = 2     // Might change to crouching if I want to do that
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private PlayerState playerState;

    public PlayerState PlayerMovementState
    {
        get { return playerState; }
    }

    [SerializeField]
    private float maxStamina;
    private float stamina;
    [SerializeField]
    private float staminaConsumptionRate;
    [SerializeField]
    private float staminaReplenishRate;
    [SerializeField]
    private Image staminaBar;
    [SerializeField]
    private float staminaLerpPercent;

    [SerializeField]
    private float maxFlightStamina;
    private float flightStamina;
    [SerializeField]
    private float flightStaminaConsumptionRate;
    [SerializeField]
    private float flightStaminaSprintConsumptionRate;
    [SerializeField]
    private float flightStaminaReplenishRate;
    [SerializeField]
    private Image flightStaminaBar;
    [SerializeField]
    private float flightStaminaLerpPercent;

    [SerializeField]
    private float speedGround;
    [SerializeField]
    private float sprintSpeedGround;
    [SerializeField]
    private float walkSpeedGround;

    [SerializeField]
    private float oversprintCooldown = 2;
    [SerializeField]
    private float oversprintTimer = 0;

    [SerializeField]
    private float speedAir;
    [SerializeField]
    private float sprintSpeedAir;
    [SerializeField]
    private float walkSpeedAir;

    [SerializeField]
    private float currentSpeed;

    [SerializeField]
    private float speedVerticalGround;

    [SerializeField]
    private float speedVerticalAir;

    private float currentSpeedVertical;

    [SerializeField]
    private float fallingMovementSpeed;

    [SerializeField]
    private bool pressedCTRLOnce = false;
    private float dropTimer = 0.3f;
    private float dropTimerLeft = 0.0f;

    [SerializeField]
    private Vector3 velocity;

    private CharacterController characterController;

    [SerializeField]
    private bool isRotatingWithCamera = true;
    public bool IsRotatingWithCamera
    {
        get { return isRotatingWithCamera; }
    }

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float groundGravity;

    private float currentGravity;

    [SerializeField]
    private float maxDescentSlopeAngle = 80.0f;

    LayerMask landableMask;

    private Vector3 lastCollisionPoint;

    // Start is called before the first frame update
    void Start()
    { 
        playerState = PlayerState.InAir;
        characterController = GetComponent<CharacterController>();
        landableMask = LayerMask.GetMask("LandableSurface");
        currentSpeed = speedAir;
        currentSpeedVertical = speedVerticalAir;
        currentGravity = 0.0f;

        lastCollisionPoint = Vector3.zero;

        stamina = maxStamina;
        flightStamina = maxFlightStamina;
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
        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, stamina / 100f, staminaLerpPercent * Time.deltaTime);
        flightStaminaBar.fillAmount = Mathf.Lerp(flightStaminaBar.fillAmount, flightStamina / 100f, flightStaminaLerpPercent * Time.deltaTime);
    }

    // Handles player movement
    public void MovePlayer()
    {
        switch (playerState)
        {
            case PlayerState.InAir:
                AirSprint();

                velocity = new Vector3(0.0f, 0.0f, 0.0f);
                velocity += transform.forward * Input.GetAxis("Vertical") * currentSpeed;
                velocity += transform.right * Input.GetAxis("Horizontal") * currentSpeed;

                if (Input.GetKey(KeyCode.Space))
                {
                    Ascend(speedVerticalAir);
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    Descend(speedVerticalAir);
                }
                break;
            case PlayerState.OnGround:
                velocity.y = 0.0f;

                if (!characterController.isGrounded)
                {
                    playerState = PlayerState.Falling;
                    currentSpeed = fallingMovementSpeed;
                    currentGravity = gravity;
                    break;
                }

                GroundSprint();
                velocity = new Vector3(0.0f, velocity.y, 0.0f);
                velocity += transform.forward * Input.GetAxis("Vertical") * currentSpeed;
                velocity += transform.right * Input.GetAxis("Horizontal") * currentSpeed;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Ascend(speedVerticalGround);
                    playerState = PlayerState.Jumping;
                }

                velocity += Vector3.up * -currentGravity;

                if (!Input.GetKey(KeyCode.Space))
                {
                    velocity.y = DescendSlope().y - transform.position.y;
                }
                break;
            case PlayerState.Jumping:
            case PlayerState.Falling:
                velocity += Vector3.up * -currentGravity;
                velocity = new Vector3(0.0f, velocity.y, 0.0f);
                velocity += transform.forward * Input.GetAxis("Vertical") * currentSpeed;
                velocity += transform.right * Input.GetAxis("Horizontal") * currentSpeed;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if(flightStamina > 0.0f)
                    {
                        playerState = PlayerState.InAir;
                        currentSpeed = speedAir;
                        currentGravity = 0.0f;
                    }
                }
                break;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    // Handles player rotation
    public void RotatePlayer(bool isADS, Quaternion cameraRotation, float rotationSmoothing)
    {
        if(isRotatingWithCamera || isADS || characterController.velocity.magnitude >= delta)
        {
            Quaternion rotationTarget = cameraRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationTarget, rotationSmoothing);
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
    }

    // Moves player upwards (used in the air and on the ground to jump)
    private void Ascend(float verticalSpeed)
    {
        velocity += Vector3.up * Input.GetAxis("Jump") * currentSpeedVertical;
    }

    // Moves player downwards
    private void Descend(float verticalSpeed)
    {
        velocity += Vector3.up * Input.GetAxis("Jump") * currentSpeedVertical;

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!pressedCTRLOnce)
            {
                pressedCTRLOnce = true;
                dropTimerLeft = dropTimer;
            }
            else
            {
                pressedCTRLOnce = false;
                playerState = PlayerState.Falling;
                currentSpeed = fallingMovementSpeed;
                currentGravity = gravity;
            }
        }
    }

    // Detects if the player has landed on a surface that can be landed on and transitions them to the OnGround state
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Need to update once animations are added to handle taking off and landing
        if (hit.collider.gameObject.layer == 9 && characterController.isGrounded)
        {
            if (playerState != PlayerState.OnGround)
            {
                playerState = PlayerState.OnGround;
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    currentSpeed = speedGround;
                }
                else
                {
                    currentSpeed = sprintSpeedGround;
                }
                currentSpeedVertical = speedVerticalGround;
                currentGravity = gravity;
                lastCollisionPoint = hit.point - transform.position;
            }
        }
    }

    // General sprint method that handles movement speed, stamina consumption, and stamina replenishment
    private void Sprint(ref float staminaUsed, float maxStaminaUsed, float staminaUsedConsumptionRate, float staminaUsedReplenishRate, ref float staminaNotUsed, float maxStaminaNotUsed, float staminaNotUsedReplenishRate, float regularSpeed, float sprintSpeed)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            if(characterController.velocity.magnitude > 0.0f)
            {
                staminaUsed -= staminaUsedConsumptionRate * Time.deltaTime;
            }
            else
            {
                ReplenishStamina(ref staminaUsed, maxStaminaUsed, staminaUsedReplenishRate);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = regularSpeed;
        }
        ReplenishStamina(ref staminaNotUsed, maxStaminaNotUsed, staminaNotUsedReplenishRate);
    }

    // Calls sprint method and handles sprinting behavior specific to the OnGround state
    private void GroundSprint()
    {
        if(oversprintTimer <= 0.0f)
        {
            Sprint(ref stamina, maxStamina, staminaConsumptionRate, staminaReplenishRate, ref flightStamina, maxFlightStamina, flightStaminaReplenishRate, speedGround, sprintSpeedGround);
            if(stamina <= 0.0f)
            {
                stamina = 0.0f;
                oversprintTimer = oversprintCooldown;
                currentSpeed = speedGround;
            }
        }
        
        else
        {
            oversprintTimer -= Time.deltaTime;
            ReplenishStamina(ref stamina, maxStamina, staminaReplenishRate);
            ReplenishStamina(ref flightStamina, maxFlightStamina, flightStaminaReplenishRate);      // Necessary because the Sprint method usually replenishes stamina. If Sprint can't be called, ground stamina and flight stamina cannot regenerate
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            ReplenishStamina(ref stamina, maxStamina, staminaReplenishRate);
        }
    }

    // Calls sprint method and handles sprinting behavior specific to the InAir state
    private void AirSprint()
    {
        if(flightStamina > 0.0f)
        {
            Sprint(ref flightStamina, maxFlightStamina, flightStaminaSprintConsumptionRate, flightStaminaReplenishRate, ref stamina, maxStamina, staminaReplenishRate, speedAir, sprintSpeedAir);
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                flightStamina -= flightStaminaConsumptionRate * Time.deltaTime;
            }
            if (flightStamina <= 0.0f)
            {
                Debug.Log("Falling");
                flightStamina = 0.0f;
                playerState = PlayerState.Falling;
                currentSpeed = fallingMovementSpeed;
                currentGravity = gravity;
                velocity.y = 0.0f;
            }
        }
    }

    // Replenishes a given stamina type by a given replenishment rate up to a given max
    private void ReplenishStamina(ref float stamina, float maxStamina, float staminaReplenishRate)
    {
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        else if (stamina < maxStamina)
        {
            stamina += staminaReplenishRate * Time.deltaTime;
        }
    }

    // Detects slope of the surface a player is moving on and sticks the player to the surface to create a smooth descent.
    // Note: This algorithm doesn't like negative y values. I could spend time trying to fix it, or I could just raise every level by 100 units and the problem goes away.
    // Clarification: I'm not fixing this, just raise the level by 100 units.
    private Vector3 DescendSlope()
    {
        float directionZ = Mathf.Sign(velocity.z);
        float directionX = Mathf.Sign(velocity.x);

        RaycastHit hit = new RaycastHit();
        Vector3 output = Vector3.zero;

        Vector3 pointOnCollider = transform.position + velocity + lastCollisionPoint;
        hit = RaycastToGround(pointOnCollider);

        if (hit.distance > delta)
        {
            float angle = Vector3.Angle(hit.normal, transform.up);
            if (angle != 0.0f && angle <= maxDescentSlopeAngle)
            {
                if(Mathf.Sign(hit.normal.z) == Mathf.Sign(velocity.z) && Mathf.Sign(hit.normal.x) == Mathf.Sign(velocity.x))
                {
                    output.y = hit.point.y;
                }
            }
        }
        return output;
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
