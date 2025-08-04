using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField]
    Transform start = null;

    [SerializeField]
    Transform target = null;


    [SerializeField]
    float yOffset = -5f;

    [SerializeField]
    float movingSpeed = 5f, sinkingSpeed = 3f;

    [SerializeField]
    float fadeSpeed = 1f;

    private enum MovementState { MovingToTarget, MovingToEndPosition, MovingToStartPosition }
    private MovementState currentState;

    Renderer boatRenderer;
    Material boatMaterial;
    Parenter parenter;

    Vector3 positionAtStartframe;
    bool isMoving;

    private void Start()
    {
        positionAtStartframe = transform.position;
        parenter = GetComponent<Parenter>();
        StartBoat();


        if (boatRenderer == null)
            boatRenderer = GetComponentInChildren<Renderer>();

        boatMaterial = boatRenderer.material;
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        switch (currentState)
        {
            case MovementState.MovingToTarget:
                MoveToTarget();
                break;
            case MovementState.MovingToEndPosition:
                MoveToEndPosition();
                break;
            case MovementState.MovingToStartPosition:
                MoveToStartPosition();
                break;
        }
    }

    private void MoveToTarget()
    {
        if (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, movingSpeed * Time.fixedDeltaTime);
        }
        else
        {
            currentState = MovementState.MovingToEndPosition;
        }
    }

    private void MoveToEndPosition()
    {
        Vector3 targetWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        if (Vector3.Distance(transform.position, targetWithOffset) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWithOffset, sinkingSpeed * Time.fixedDeltaTime);

            FadeTo(0, fadeSpeed);
        }
        else
        {
            SetFade(0);
            parenter.ForceUnestAll();
            transform.position = new Vector3(start.position.x, start.position.y + yOffset, start.position.z);
            currentState = MovementState.MovingToStartPosition;
        }
    }

    private void MoveToStartPosition()
    {
        if (Vector3.Distance(transform.position, start.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, start.position, sinkingSpeed * Time.fixedDeltaTime);

            FadeTo(1, fadeSpeed);
        }
        else
        {
            SetFade(1);
            currentState = MovementState.MovingToTarget;
        }
    }

    public void ResetBoat()
    {
        isMoving = false;
        transform.position = positionAtStartframe;
    }

    public void StartBoat()
    {
        if (!isMoving)
        {
            isMoving = true;
            currentState = MovementState.MovingToTarget;
        }
        else
        {
            Debug.LogWarning("BoatStart was called, but Boat is already moving.");
        }
    }

    void OnDestroy()
    {
        isMoving = false;
        //StopAllCoroutines();
    }



    private void FadeTo(float targetFade, float fadeSpeed)
    {
        Color color = boatMaterial.color;
        color.a = Mathf.Lerp(color.a, targetFade, fadeSpeed * Time.fixedDeltaTime);
        boatMaterial.color = color;
    }

    void SetFade(float targetValue)
    {
        Color color = boatMaterial.color;
        color.a = targetValue;
        boatMaterial.color = color;
    }
}
