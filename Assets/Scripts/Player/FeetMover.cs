using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeetMover : MonoBehaviour
{
    [SerializeField]
    Transform leftFoot = null;

    [SerializeField]
    Transform rightFoot = null;

    [SerializeField]
    Color onGroundColor, inAirColor;

    [SerializeField]
    float walkStepDistance = 0.8f, walkStepSpeed = 10f, sprintStepDistance = -1f, sprintStepSpeed = 20f;
    float currentStepSpeed;
    float currentStepDistance;

    [SerializeField, Range(0, 1)]
    float shortenSideMovementFactor = 0.35f;

    bool isLeftFootTurn;
    bool isInCoroutine;
    bool areFeetBlocked;

    bool isInReset;

    Vector3 stepDirection;

    private SpriteRenderer leftFootRenderer, rightFootRenderer;
    Vector3 leftLocalOrigin, rightLocalOrigin;

    PlayerInput playerInput;
    InputAction move, sprint;


    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
        move = playerInput.Player.Move;
        sprint = playerInput.Player.Sprint;

        leftLocalOrigin = leftFoot.localPosition;
        rightLocalOrigin = rightFoot.localPosition;

        leftFootRenderer = leftFoot.GetComponentInChildren<SpriteRenderer>();
        rightFootRenderer = rightFoot.GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        move.Enable();
        sprint.Enable();

        move.started += OnMovement;

        PlayerMovement.OnIsInAir += OnMoveCanceled;
        PlayerMovement.OnIsInAir += OnMovementBlocked;
        PlayerMovement.OnIsInAir += ChangeColorToIsInAirColor;

        PlayerMovement.OnHasLanded += OnLanding;
        PlayerMovement.OnHasLanded += ChangeColorToGroundColor;
    }

    private void OnDisable()
    {
        move.Disable();
        sprint.Disable();

        move.started -= OnMovement;

        PlayerMovement.OnIsInAir -= OnMoveCanceled;
        PlayerMovement.OnIsInAir -= OnMovementBlocked;
        PlayerMovement.OnIsInAir -= ChangeColorToIsInAirColor;


        PlayerMovement.OnHasLanded -= OnLanding;
        PlayerMovement.OnHasLanded -= ChangeColorToGroundColor;
    }

    private void OnMovement(InputAction.CallbackContext callbackContext)
    {
        MoveFeet();
    }

    void OnMoveCanceled()
    {
        StopAllCoroutines();

        StartCoroutine(ResetFeet(leftFoot, rightFoot, leftLocalOrigin, rightLocalOrigin));
    }

    void OnMovementBlocked()
    {
        areFeetBlocked = true;
    }

    void OnLanding()
    {
        StopAllCoroutines();
        isInCoroutine = false;

        areFeetBlocked = false;

        MoveFeet();
    }

    void MoveFeet()
    {
        if (isInCoroutine || areFeetBlocked) return;

        isInCoroutine = true;

        Transform currentFoot = isLeftFootTurn ? leftFoot : rightFoot;
        Transform nextFoot = isLeftFootTurn ? rightFoot : leftFoot;
        SpriteRenderer currentRenderer = isLeftFootTurn ? leftFootRenderer : rightFootRenderer;
        SpriteRenderer nextRenderer = isLeftFootTurn ? rightFootRenderer : leftFootRenderer;

        Vector3 currentOriginalPosition = isLeftFootTurn ? leftLocalOrigin : rightLocalOrigin;
        Vector3 nextOriginalPosition = isLeftFootTurn ? rightLocalOrigin : leftLocalOrigin;

        StartCoroutine(MoveFirstFoot(currentFoot, nextFoot, currentRenderer, nextRenderer, currentOriginalPosition, nextOriginalPosition));
    }

    IEnumerator MoveFirstFoot(Transform currentFoot, Transform nextFoot, SpriteRenderer currentRenderer, SpriteRenderer nextRenderer, Vector3 currentOriginalPosition, Vector3 nextOriginalPosition)
    {
        if (isInReset)
            StopCoroutine("ResetFeet");

        while (move.IsPressed())
        {
            //Update Information
            currentStepSpeed = sprint.IsPressed() ? sprintStepSpeed : walkStepSpeed;
            currentStepDistance = sprint.IsPressed() ? sprintStepDistance : walkStepDistance;

            stepDirection = new Vector3(move.ReadValue<Vector2>().x, 0f, move.ReadValue<Vector2>().y).normalized * -1; //*-1 for animation reasons
            stepDirection.x *= shortenSideMovementFactor;



            Vector3 targetPosition = currentOriginalPosition + (stepDirection * currentStepDistance);
            Vector3 nextTargetPosition = nextOriginalPosition + (stepDirection * currentStepDistance);

            currentRenderer.color = new Color(inAirColor.r, inAirColor.g, inAirColor.b, inAirColor.a);
            nextRenderer.color = new Color(onGroundColor.r, onGroundColor.g, onGroundColor.b, onGroundColor.a);
            // current goes forward
            // next goes back/stays
            while (Vector3.Distance(currentFoot.localPosition, currentOriginalPosition) > 0.01f)
            {
                currentFoot.localPosition = Vector3.MoveTowards(currentFoot.localPosition, currentOriginalPosition, currentStepSpeed * Time.deltaTime);
                yield return nextFoot.localPosition = Vector3.MoveTowards(nextFoot.localPosition, nextTargetPosition, currentStepSpeed * Time.deltaTime);
            }
            currentFoot.localPosition = currentOriginalPosition;
            nextFoot.localPosition = nextTargetPosition;


            currentRenderer.color = new Color(onGroundColor.r, onGroundColor.g, onGroundColor.b, onGroundColor.a);
            nextRenderer.color = new Color(inAirColor.r, inAirColor.g, inAirColor.b, inAirColor.a);
            // current goes back
            // next goes forward
            while (Vector3.Distance(currentFoot.localPosition, targetPosition) > 0.01f)
            {
                currentFoot.localPosition = Vector3.MoveTowards(currentFoot.localPosition, targetPosition, currentStepSpeed * Time.deltaTime);
                yield return nextFoot.localPosition = Vector3.MoveTowards(nextFoot.localPosition, nextOriginalPosition, currentStepSpeed * Time.deltaTime);
            }
            currentFoot.localPosition = targetPosition;
            nextFoot.localPosition = nextOriginalPosition;


            yield return null;
            // and so on and so on...
        }

        isInCoroutine = false;
        isLeftFootTurn = !isLeftFootTurn;  // animation reason
        OnMoveCanceled();

        ChangeColorToGroundColor();
    }

    //RESET FEET
    IEnumerator ResetFeet(Transform foot, Transform nextFoot, Vector3 originalPosition, Vector3 nextOriginalPosition)
    {
        isInReset = true;

        while (Vector3.Distance(foot.localPosition, originalPosition) > 0.01f)
        {
            foot.localPosition = Vector3.MoveTowards(foot.localPosition, originalPosition, currentStepSpeed * Time.deltaTime);
            nextFoot.localPosition = Vector3.MoveTowards(nextFoot.localPosition, nextOriginalPosition, currentStepSpeed * Time.deltaTime);
            yield return null;
        }
        foot.localPosition = originalPosition;
        nextFoot.localPosition = nextOriginalPosition;

        isInReset = false;

    }

    // Maybe helpful
    IEnumerator SetFootTo(Transform foot, Vector3 originalPosition)
    {
        while (Vector3.Distance(foot.localPosition, originalPosition) > 0.01f)
        {
            foot.localPosition = Vector3.Lerp(foot.localPosition, originalPosition, currentStepSpeed * Time.deltaTime);
            yield return null;
        }
        foot.localPosition = originalPosition;
    }

    void ChangeColorToGroundColor()
    {
        leftFootRenderer.color = new Color(onGroundColor.r, onGroundColor.g, onGroundColor.b, onGroundColor.a);
        rightFootRenderer.color = new Color(onGroundColor.r, onGroundColor.g, onGroundColor.b, onGroundColor.a);
    }

    void ChangeColorToIsInAirColor()
    {
        leftFootRenderer.color = new Color(inAirColor.r, inAirColor.g, inAirColor.b, inAirColor.a);
        rightFootRenderer.color = new Color(inAirColor.r, inAirColor.g, inAirColor.b, inAirColor.a);
    }
}
