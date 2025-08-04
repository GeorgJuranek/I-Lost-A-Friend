using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDisplay;
    [SerializeField] private TMP_InputField messageInputField;

    private DatabaseReference databaseReference;
    bool hasSubmited;

    public static Action OnFetchWasSuccessful;
    public static Action OnHasSuccessfulSubmited;

    private void Start()
    {
        StartCoroutine(WaitForFirebaseInitialization());
    }

    private IEnumerator WaitForFirebaseInitialization()
    {
        while (FirebaseManager.Instance == null)
        {
            Debug.Log("Waiting for FirebaseManager instance...");
            yield return new WaitForSeconds(0.5f);
        }

        while (FirebaseManager.Instance.databaseReference == null)
        {
            Debug.Log("Waiting for databaseReference to be initialized...");
            yield return new WaitForSeconds(0.5f);
        }

        databaseReference = FirebaseManager.Instance.databaseReference;
        Debug.Log("MessageManager: DatabaseReference assigned!");

        FetchMessages();
    }

    public void OnSendMessageButtonClicked()
    {
        SendFirebaseMessage(messageInputField.text);
    }

    private void SendFirebaseMessage(string messageText)
    {
        if (string.IsNullOrEmpty(messageText))
        {
            Debug.Log("Message cannot be empty!");
            return;
        }

        if (hasSubmited) return;


        long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; // DEBUG

        Dictionary<string, object> messageData = new Dictionary<string, object>()
        {
            { "message", messageText },
            { "timestamp", unixTimestamp }
        };

        string json = JsonConvert.SerializeObject(messageData);
        Debug.Log("Debug JSON Output: " + json);

        if (databaseReference == null)
        {
            Debug.LogWarning("While trying fetch, NO databaseReference was found!");
            return;
        }

        databaseReference.Child("messages").Push().SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to send message: {task.Exception}");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Message sent successfully!");
                    FetchMessages();
                }
            });


        hasSubmited = true;
        messageInputField.text = "";
        OnHasSuccessfulSubmited?.Invoke();
    }

    private void FetchMessages()
    {
        if (databaseReference==null)
        {
            Debug.LogWarning("While trying fetch, NO databaseReference was found!");
                return;
        }


        databaseReference.Child("messages").OrderByChild("timestamp").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to fetch messages: {task.Exception}");
                return;
            }
            if (task.IsCanceled)
            {
                Debug.LogError("Fetching messages was canceled.");
                return;
            }

            try
            {
                DataSnapshot snapshot = task.Result;
                string messageText = "";
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    messageText += $"\n{childSnapshot.Child("message").Value}\n";

                }

                messageDisplay.text = messageText;

                Debug.Log("MESSAGETEXT:  " + messageText);

                OnFetchWasSuccessful?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing snapshot: {e}");
            }
        });
    }

    public void MenuFetch()
    {
        messageDisplay.text = "Loading ....";
        FetchMessages();
    }

    public void GoToUI(GameObject focus)
    {
        if (focus == null || EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(focus);
    }

}

