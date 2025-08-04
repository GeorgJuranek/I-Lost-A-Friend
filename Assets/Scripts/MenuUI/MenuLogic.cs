using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField]
    GameObject CreditsBox;

    [SerializeField]
    GameObject ControlsBox;

    [SerializeField]
    GameObject QuitingBox;

    [SerializeField]
    GameObject SettingsBox;

    [SerializeField]
    Button defaultStartButton;

    GameObject lastSelected;


    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (defaultStartButton != null)
            EventSystem.current.SetSelectedGameObject(defaultStartButton.gameObject);

        Time.timeScale = 0;
    }

    public void OnResume()
    {
        if (lastSelected!=null)
        {
            lastSelected.SetActive(false);
            lastSelected = null;
        }

        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }

    public void OnControls()
    {
        if (lastSelected != null && lastSelected != ControlsBox)
        {
            lastSelected.SetActive(false);
            lastSelected = null;
        }

        ControlsBox.SetActive(!ControlsBox.activeSelf);

        lastSelected = (ControlsBox.activeSelf ? ControlsBox : null);

    }

    public void OnCredits()
    {

        if (lastSelected != null && lastSelected != CreditsBox)
        {
            lastSelected.SetActive(false);
            lastSelected = null;
        }

        CreditsBox.SetActive(!CreditsBox.activeSelf);

        lastSelected = (CreditsBox.activeSelf ? CreditsBox : null);

    }

    public void OnQuitSelection()
    {

        if (lastSelected != null && lastSelected!=QuitingBox)
        {
            lastSelected.SetActive(false);
            lastSelected = null;
        }

        QuitingBox.SetActive(!QuitingBox.activeSelf);

        lastSelected = (QuitingBox.activeSelf ? QuitingBox : null);

    }


    public void OnSettings()
    {
        if (lastSelected != null && lastSelected != SettingsBox)
        {
            lastSelected.SetActive(false);
            lastSelected = null;
        }

        SettingsBox.SetActive(!SettingsBox.activeSelf);

        lastSelected = (SettingsBox.activeSelf ? SettingsBox : null);

    }

    public void OnQuit()
    {
        Debug.Log("Application was quit");
        Application.Quit();
    }

    private void OnDisable()
    {
        OnResume();
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene("Transmission0");
    }

    public void GoToUI(GameObject focus)
    {
        if (focus.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(focus);
    }

}
