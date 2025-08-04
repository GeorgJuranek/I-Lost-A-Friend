using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    PlayerInput input;
    InputAction mouseLook;
    InputAction controllerLook;

    Vector3 currentRotation;

    [SerializeField]
    float mouseSpeed = 40f;

    public float MouseSpeed { get => mouseSpeed; set { mouseSpeed = value; } }

    bool isUsingController;

    bool isLocked = false;

    void Awake()
    {
        input = new PlayerInput();
        mouseLook = input.Player.Look;
        controllerLook = input.Player.ControllerLook;

        currentRotation = transform.localEulerAngles;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        mouseLook.Enable();
        controllerLook.Enable();
    }

    private void OnDisable()
    {
        mouseLook.Disable();
        controllerLook.Disable();
    }

    void Update()
    {
        if (isLocked) return;

        Vector2 mouseInput = mouseLook.ReadValue<Vector2>();
        Vector2 controllerInput = controllerLook.ReadValue<Vector2>();

        if (mouseInput.magnitude > 0f)
            ApplyLook(mouseInput);

        else if (controllerInput.magnitude > 0f)
            ApplyLook(controllerInput * 5f);
    }

    private void ApplyLook(Vector2 input)
    {
        currentRotation += new Vector3(-input.y, input.x, 0f) * mouseSpeed * Time.deltaTime;
        currentRotation.x = Mathf.Clamp(currentRotation.x, -90, 90);

        transform.localEulerAngles = currentRotation;
    }

}
