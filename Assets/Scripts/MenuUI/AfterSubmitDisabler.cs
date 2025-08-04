using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AfterSubmitDisabler : MonoBehaviour
{
    [SerializeField]
    Button button;

    [SerializeField]
    TMP_InputField inputfield;

    [SerializeField]
    GameObject text;


    private void OnEnable()
    {
        MessageManager.OnHasSuccessfulSubmited += DisableElements;
    }


    private void OnDisable()
    {
        MessageManager.OnHasSuccessfulSubmited -= DisableElements;
    }


    private void DisableElements()
    {
        button.interactable = false;
        inputfield.interactable = false;
        text.SetActive(false);
    }
}
