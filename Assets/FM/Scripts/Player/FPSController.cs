using UnityEngine;
using UnityEngine.VFX;

public enum State
{
    Normal,
    GHookThrown,
    GHookFlying,
}

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Basic Parameters")]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float jumpPower = 30f;
    [SerializeField] private float lookSpeed = 2.3f;
    [SerializeField] private float gravity = 40f;

    [Header("Grappling Hook Parameters")]
    [SerializeField] private float gHookRotationSpeed = 10f;
    [SerializeField] private float gHookMaxReach = 100f;
    [SerializeField] private float reachedGHookPositionDistance = 2f;
    [SerializeField] float gHookFlySpeedMin = 10f;
    [SerializeField] float gHookFlySpeedMax = 45f;
    [SerializeField] private float gHookThrowBaseSpeed = 100f;
    [SerializeField] private float gHookFlySpeedMultiplier = 2f;
    [SerializeField] private float gHookFov = 75f;
    [SerializeField] private float momentumDrag = 5f;
    [SerializeField] private float extraJumpMomentum = 3f;
    [SerializeField] private LayerMask hookableLayers;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Gun gun;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CameraFieldOfVision cameraFov;
    [SerializeField] private Transform gHookTransform;
    [SerializeField] private Transform gHookPoint;
    [SerializeField] private VisualEffect speedLinesVisualEffect;

    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;

    private State currentState;
    private Vector3 gHookPosition;
    private float gHookFlySpeed;
    private float gHookIDistanceTravelled;
    private Quaternion gunStartingRotation;
    private bool lastGHookHit;
    private bool allowInput = false;

    public Vector3 GHookPosition => gHookPosition;
    public State CurrentState => currentState;
    public Transform GunPoint => gHookPoint;

    void Awake()
    {
        currentState = State.Normal;
        gunStartingRotation = gHookTransform.localRotation;
    }

    void Update()
    {
        HandleCharacterLook();
        HandleInputFire();
        switch (currentState)
        {
            default:
            case State.Normal:
                HandleGHookAim();
                HandleCharacterMovement();
                HandleGHookStart();
                break;
            case State.GHookThrown:
                HandleCharacterMovement();
                HandleGHookThrow();
                break;
            case State.GHookFlying:
                HandleGHookMovement();
                break;
        }
    }

    public void AllowInput(bool allowInput)
    {
        this.allowInput = allowInput;
    }

    private void ResetGravity()
    {
        characterVelocityY = 0f;
    }

    private void HandleCharacterMovement()
    {
        Vector2 movementVector = TestInputMovement();
        Vector3 characterVelocity = transform.right * movementVector.x * walkSpeed + transform.forward * movementVector.y * walkSpeed;

        if (characterController.isGrounded)
        {
            characterVelocityY = 0f;
            if (TestInputJump())
            {
                characterVelocityY = jumpPower;
            }
        }

        characterVelocityY -= gravity * Time.deltaTime;
        characterVelocity.y = characterVelocityY;
        characterVelocity += characterVelocityMomentum;

        characterController.Move(characterVelocity * Time.deltaTime);

        if (characterVelocityMomentum.magnitude >= 0f)
        {
            momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }
    }

    private void HandleCharacterLook()
    {
        Vector2 mouseVector = TestInputMouse();

        transform.Rotate(new Vector3(0f, mouseVector.x * lookSpeed, 0f), Space.Self);
        cameraVerticalAngle -= mouseVector.y * lookSpeed;

        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -85f, 85f);

        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0f, 0f);
    }

    private void HandleGHookAim()
    {
        gHookTransform.localRotation = Quaternion.Lerp(gHookTransform.localRotation, gunStartingRotation, Time.deltaTime * gHookRotationSpeed);
    }

    private void HandleGHookStart()
    {
        if (TestInputGHook())
        {
            gHookIDistanceTravelled = 0f;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit, gHookMaxReach))
            {
                gHookPosition = raycastHit.point;
                lastGHookHit = true;
            }
            else
            {
                gHookPosition = playerCamera.transform.position +  playerCamera.transform.forward * gHookMaxReach;
                lastGHookHit = false;
            }
            currentState = State.GHookThrown;
        }
    }

    private void HandleGHookThrow()
    {
        gHookTransform.LookAt(gHookPosition);
        gHookIDistanceTravelled += gHookThrowBaseSpeed * Time.deltaTime;
        if (gHookIDistanceTravelled >= Vector3.Distance(transform.position, gHookPosition))
        {
            if (lastGHookHit == false)
            {
                StopGHook();
            }
            else
            {
                currentState = State.GHookFlying;
                cameraFov.SetCameraFov(gHookFov);
                speedLinesVisualEffect.gameObject.SetActive(true);
            }
        }
    }

    private void HandleGHookMovement()
    {
        gHookTransform.LookAt(gHookPosition);
        Vector3 gHookDir = (gHookPosition - transform.position).normalized;
        gHookFlySpeed = Mathf.Clamp(Vector3.Distance(transform.position, gHookPosition) * gHookFlySpeedMultiplier, gHookFlySpeedMin, gHookFlySpeedMax);
        characterController.Move(gHookDir * gHookFlySpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, gHookPosition) < reachedGHookPositionDistance)
        {
            ResetGravity();
            StopGHook();
        }

        if (TestInputJump())
        {
            characterVelocityMomentum = gHookDir * gHookFlySpeed;
            characterVelocityMomentum += Vector3.up * jumpPower * extraJumpMomentum;
            ResetGravity();
            StopGHook();
        }

        if (TestInputGHook())
        {
            StopGHook();
        }
    }

    private void StopGHook()
    {
        cameraFov.ResetCameraFov();
        speedLinesVisualEffect.gameObject.SetActive(false);
        currentState = State.Normal;
    }

    private Vector2 TestInputMovement()
    {
        Vector2 movementVector = Vector2.zero;
        if (allowInput)
        {
            movementVector.x = Input.GetAxisRaw("Horizontal");
            movementVector.y = Input.GetAxisRaw("Vertical");
        }
        return movementVector;
    }

    private Vector2 TestInputMouse()
    {
        Vector2 mouseVector = Vector2.zero;
        if (allowInput)
        {
            mouseVector.x = Input.GetAxisRaw("Mouse X");
            mouseVector.y = Input.GetAxisRaw("Mouse Y");
        }
        return mouseVector;
    }

    private bool TestInputGHook()
    {
        return allowInput && Input.GetKeyDown(KeyCode.E);
    }

    private bool TestInputJump()
    {
        return allowInput && Input.GetKeyDown(KeyCode.Space);
    }

    private void HandleInputFire() 
    {
        if (allowInput)
        {
            gun.HandleGunInput();
        }
    }
}