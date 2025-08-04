using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCaller : MonoBehaviour
{
    [SerializeField]
    GameObject targetCanvas;

    PlayerInput playerInput;
    InputAction menu;


    void Awake()
    {
        playerInput = new PlayerInput();

        menu = playerInput.Player.Menu;
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();

        menu.Enable();

        menu.started += OnMenuCall;
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();

        menu.Disable();

        menu.started -= OnMenuCall;
    }

    public void OnMenuCall(InputAction.CallbackContext callbackContext)
    {
        targetCanvas.SetActive(!targetCanvas.activeSelf);
    }
}
