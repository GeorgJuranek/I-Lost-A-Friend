using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Holder : MonoBehaviour
{
    InputAction hold;
    InputAction look;
    InputAction controllerLook;

    PlayerInput playerInput;

    [SerializeField]
    [Range(0, 10)]
    int rayLength;

    [SerializeField]
    Joint joint;

    [SerializeField]
    float speedToPosition;

    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    GameObject checkerCollider;

    CollisionLogic collisioner;

    bool targetInRange;
    public bool TargetInRange
    {
        private get => targetInRange;
        set
        {
            if (value != targetInRange)
            {
                OnHoldableInRange?.Invoke(value);
            }

            targetInRange = value;
        }
    }

    public Action<bool> OnHoldableInRange;

    IGrabable grab = null;
    public IGrabable Grab { get => grab; private set { grab = value; } }

    bool isInRightDistance;
    public bool IsInRightDistance
    {
        private get => isInRightDistance;
        set
        {
            if (value != isInRightDistance)
            {
                OnRightDistance?.Invoke(value);
            }
    
            isInRightDistance = value;
        }
    }
    public Action<bool> OnRightDistance;

    private void Awake()
    {
        playerInput = new PlayerInput();
        hold = playerInput.Player.Hold;
        look = playerInput.Player.Look;
        controllerLook = playerInput.Player.ControllerLook;

        collisioner = GetComponent<CollisionLogic>();
    }

    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, transform.forward*rayLength, Color.red);

        if (grab != null)
        {
            SetRotationToPlayerRotation(grab.ReflectSelf(), grab.getHoldingRotation());

            LetGoWhenCollision();
        }


    }

    private void FixedUpdate()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }

        //Check if IGrabable is in range 
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, rayLength) && grab==null)
        {

            if (hitInfo.collider.gameObject.TryGetComponent<IGrabable>(out IGrabable checkGrabInRange))//Only for checking
            {
                TargetInRange = true;

            }
            else if (hitInfo.collider.gameObject.GetComponentInChildren<IGrabable>() != null)
            {

                GameObject trueGrabable = hitInfo.collider.gameObject.GetComponentInChildren<IGrabable>().ReflectSelf();

                if (hitInfo.collider.gameObject == trueGrabable)
                    TargetInRange = true;
                else
                    TargetInRange = false;

            }
            else
            {
                TargetInRange = false;
            }

        }
        else
        {
            TargetInRange = false;
        }

        if(targetInRange) //is holder in a collisionfree spot?
        {
            if (collisioner.IsInCollision)
            {
                IsInRightDistance = false;
            }
            else
            {
                IsInRightDistance = true;
            }
        }

    }

    private void OnEnable()
    {
        hold.Enable();
        look.Enable();
        controllerLook.Enable();
        hold.performed += OnHold;
    }

    private void OnDisable()
    {
        hold.performed -= OnHold;
        controllerLook.Disable();
        look.Disable();
        hold.Disable();
    }

    private void OnHold(InputAction.CallbackContext callbackContext)
    {

        if (grab == null)
        {
            if (!TargetInRange) return;

            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, rayLength) && TargetInRange && isInRightDistance)
            {
                //Find grabable
                if (hitInfo.collider.gameObject.TryGetComponent<IGrabable>(out grab))
                { }
                else if (hitInfo.collider.gameObject.GetComponentInChildren<IGrabable>() != null)
                {
                    grab = hitInfo.collider.gameObject.GetComponentInChildren<IGrabable>();
                }

                //Hold grabable
                if (grab != null)
                {
                    grab.OnGrab();
                    Hold(grab);
                }
                else
                {
                    Debug.LogWarning("Nothing to grab!");
                }
            }


        }
        else
        {
            Drop();
        }
    }

    private void Hold(IGrabable toHold)
    {
        StartCoroutine(MoveToPositionAndAttach(toHold.ReflectSelf(), toHold.getHoldingRotation()));
    }

    public static Action OnDropDenied;
    public static Action OnDropSuccess;

    public void Drop()
    {
        if (GetComponent<GroundChecker>().GroundCheckFromOtherGameObjectSphereCollider(checkerCollider) == true) //checks if holder is in a wall, so it can't be droped
        {
            OnDropDenied?.Invoke();
            return;
        }

        if (grab != null)
        {
            OnDropSuccess?.Invoke();
            grab.OnGrabExit();
            grab = null;
        }

        joint.connectedBody = null;
    }


    IEnumerator MoveToPositionAndAttach( GameObject movingObject, Vector3 targetRotation)
    {
        float distance = Vector3.Distance(movingObject.transform.position, transform.position);

        if (movingObject.TryGetComponent<Rigidbody>(out Rigidbody hasRigidBody))
            hasRigidBody.isKinematic = true;

        while (distance > 0.1f)
        {
            movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, transform.position, speedToPosition * Time.deltaTime);

            distance = Vector3.Distance(movingObject.transform.position, transform.position);

            yield return null;
        }
        movingObject.transform.position = transform.position;

        if(grab != null)
        {
            joint.connectedBody = grab.ReflectSelf().GetComponent<Rigidbody>();
            grab.OnGrab();
        }
    }

    private void SetRotationToPlayerRotation(GameObject rotatingObject, Vector3 targetRotation)
    {
        Quaternion targetQuaternion = Quaternion.Euler(transform.eulerAngles.x + targetRotation.x, transform.eulerAngles.y + targetRotation.y, transform.eulerAngles.z + targetRotation.z);

        if (Quaternion.Angle(rotatingObject.transform.rotation, targetQuaternion) < 5f)
        {
            return;
        }

        rotatingObject.transform.rotation = Quaternion.RotateTowards(targetQuaternion, targetQuaternion, rotationSpeed * Time.deltaTime);
    }

    private void LetGoWhenCollision()
    {
        if (collisioner.IsInCollision)
        {
            Drop();
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
