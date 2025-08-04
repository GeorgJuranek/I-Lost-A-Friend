using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TextController : MonoBehaviour
{
    [SerializeField]
    TMP_Text textComponent;

    [SerializeField]
    List<string> texts;

    [SerializeField]
    string followingSceneName;

    int currentIndex = 0;

    PlayerInput playerInput;

    InputAction confirm;


    void Awake()
    {
        playerInput = new PlayerInput();
        confirm = playerInput.Player.MenuConfirm;

        textComponent.text = texts[currentIndex];
    }

    private void OnEnable()
    {
        confirm.Enable();
        confirm.canceled += ChangeText;
    }

    private void OnDisable()
    {
        confirm.Disable();
        confirm.canceled -= ChangeText;
    }

    private void ChangeText(InputAction.CallbackContext context)
    {
        currentIndex++;

        if (currentIndex < texts.Count)
            textComponent.text = texts[currentIndex];
        else
            SceneManager.LoadScene(followingSceneName);
    }

}
