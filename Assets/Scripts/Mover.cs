using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 0.55f;

    [SerializeField]
    float moveHomeSpeed = 1f;

    public enum ETargets
    {
        None = -1,
        Up,
        Down,
        Right,
        Left,
    }

    Vector3 startPosition;
    Coroutine coroutine = null;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void Move(ETargets targetDirection, float distance)
    {
        if (coroutine != null) return;

        Vector3 movePosition = GetDirectionFromEnum(targetDirection) * distance;

        Vector3 targetHeight = transform.position + movePosition;

        coroutine = StartCoroutine(MoveTo(targetHeight, moveSpeed));
    }


    public void MoveToPosition(Vector3 targetPoint)
    {
        if (coroutine != null) return;

        coroutine = StartCoroutine(MoveTo(targetPoint, moveSpeed));
    }


    public Coroutine HandOutMoveToPositionCoroutine(Vector3 targetPoint) //So a Coroutine from this class can be controlled from outside
    {
        if (coroutine != null) return coroutine;
    
        return coroutine = StartCoroutine(MoveTo(targetPoint, moveSpeed));
    }


    public void MoveToPosition(Vector3 targetDirection, float distance)
    {
        if (coroutine != null) return;

        coroutine = StartCoroutine(MoveTo(targetDirection * distance, moveSpeed));
    }


    public void MoveToPosition(Transform target)
    {
        if (coroutine != null) return;

        Vector3 targetPosition = target.position;
        coroutine = StartCoroutine(MoveTo(targetPosition, moveSpeed));
    }

    public void ComeBackHome()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(MoveTo(startPosition, moveHomeSpeed));
    }


    IEnumerator MoveTo(Vector3 targetPosition, float currentMoveSpeed)
    {
        if(Vector3.Distance(transform.position, targetPosition)<1f)// check if necessary
            Debug.LogWarning("Mover target was really short distance. Are you sure this is correct?");

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);
            yield return null;
        }

        coroutine = null;
    }


    //Translator
    Vector3 GetDirectionFromEnum(ETargets targetDirection)
    {
        Vector3 target = Vector3.zero;

        switch (targetDirection)
        {
            case ETargets.Up:
                target = Vector3.up;
                break;

            case ETargets.Down:
                target = Vector3.down;
                break;

            case ETargets.Right:
                target = Vector3.right;
                break;

            case ETargets.Left:
                target = Vector3.left;
                break;

            default:
                Debug.LogWarning("default switch case in Mover script.");
                break;
        }


        return target;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
