using System;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerRigidbody;
    PlayerInput playerInput;
    InputAction move;
    InputAction jump;
    InputAction sprint;
    InputAction hold;

    [SerializeField]
    float walkSpeed = 2;

    [SerializeField]
    float sprintSpeed = 5;

    [SerializeField]
    float jumpForce = 5;

    [SerializeField]
    float playerMeshRotationSpeed;

    [SerializeField]
    GameObject groundCheckerObject = null;

    [SerializeField]
    GameObject cameraRoot = null;

    [SerializeField]
    GameObject playerMesh = null;

    [SerializeField]
    PhysicMaterial groundPhysics, airPhysics; 

    [SerializeField]
    GameObject wallCheckerObject = null;

    [SerializeField]
    float wallJumpForce = 3f;

    [SerializeField]
    GroundChecker wallcheckCollision;

    public Vector3 lastSavePosition;
    public Quaternion lastSaveRotation;
    public Quaternion LastCameraRotation { get; set; }
    public bool isUnableToMove;

    bool isGrounded = true;

    GroundChecker groundChecker;

    bool isInWallCollision = false;

    FallDetector fallDetector;

    Collider playerCollider;


    public bool IsGrounded
    {
        get => isGrounded;
        private set
        {
            if (value != isGrounded)//Value has changed
            {
                if (value) //true means must have been landed
                {
                    fallDetector.StopFalling();
                    OnHasLanded?.Invoke();
                }
                else
                {
                    if (TryGetComponent<INestable>(out INestable nestable))
                        if (nestable.TemporaryParent != null)
                            nestable.Unnest();


                    fallDetector.StartFalling();
                    OnIsInAir?.Invoke();
                }
            }

            isGrounded = value;

        }
    }
    public static Action OnHasLanded;
    public static Action OnIsInAir;

    public bool IsInWallCollision
    {
        get => isInWallCollision;
        private set
        {
            if (value != isInWallCollision)//Value has changed
            {
                if (value) 
                {
                    playerCollider.material = airPhysics;
                }
                else
                {
                    playerCollider.material = groundPhysics;
                }
            }

            isInWallCollision = value;

        }
    }

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        fallDetector = GetComponent<FallDetector>();
        playerInput = new PlayerInput();
        groundChecker = GetComponent<GroundChecker>();

        playerCollider = GetComponent<Collider>();

        move = playerInput.Player.Move;
        jump = playerInput.Player.Jump;
        sprint = playerInput.Player.Sprint;
        hold = playerInput.Player.Hold;

        lastSavePosition = transform.position;
        lastSaveRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!isUnableToMove)
        {
            Move();
            RotateToCurrentView(playerMesh);
        }

        IsGrounded = groundChecker.GroundCheckFromOtherGameObjectSphereCollider(groundCheckerObject);

        if (!IsGrounded)
            IsInWallCollision = wallcheckCollision.GroundCheckFromOtherGameObjectCapsuleCollider(wallCheckerObject);

        /*Falldetector has to detect a long fall if player is not grounded*/
        if (!isGrounded && !fallDetector.IsInCoroutine() && !isInWallCollision)
        {
            fallDetector.StartFalling();
        }
    }

    private void OnEnable()
    {
        move.Enable();
        jump.Enable();
        sprint.Enable();
        hold.Enable();

        jump.started += OnJump;


        BalloonIGrabable.OnPlayerHasReachedGoal += OnGoalReached;
    }

    private void OnDisable()
    {
        jump.started -= OnJump;

        move.Disable();
        jump.Disable();
        sprint.Disable();
        hold.Disable();


        BalloonIGrabable.OnPlayerHasReachedGoal -= OnGoalReached;
    }

    private void RotateToCurrentView(GameObject toRotateObject)
    {
        Quaternion targetRotation = Quaternion.Euler(0, cameraRoot.transform.eulerAngles.y, 0);

        if (toRotateObject.transform.localRotation != targetRotation)
        toRotateObject.transform.localRotation = Quaternion.Slerp(toRotateObject.transform.localRotation, targetRotation, playerMeshRotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (!move.IsPressed()) return;
        if (isInWallCollision) return;
        if (Time.timeScale == 0) return;

        float speed = (sprint.inProgress && !isInWallCollision) ? sprintSpeed : walkSpeed;

        Vector3 input = new Vector3(move.ReadValue<Vector2>().x, playerRigidbody.velocity.y, move.ReadValue<Vector2>().y);

        input = Quaternion.Euler(0, cameraRoot.transform.localEulerAngles.y, 0) * input;

        Vector3 speedControl = new Vector3(speed, 1, speed);
        input = new Vector3(input.x * speedControl.x, input.y * speedControl.y, input.z * speedControl.z );

        playerRigidbody.velocity = input;

    }

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (!isGrounded && !isInWallCollision) return;
        if (Time.timeScale == 0) return;


        Vector3 newVelocity = Vector3.up * (isInWallCollision ? wallJumpForce : jumpForce);

        playerRigidbody.AddForce(newVelocity, ForceMode.Impulse);

        if (playerRigidbody.velocity.y > jumpForce)
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, jumpForce, playerRigidbody.velocity.z);
        }
    }


    void OnGoalReached()
    {
        playerRigidbody.useGravity = false;
    }

}
