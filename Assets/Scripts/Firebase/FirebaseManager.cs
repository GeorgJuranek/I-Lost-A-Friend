using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private FirebaseAuth auth;
    public DatabaseReference databaseReference;

    void Awake()
    {
        // Singleton-Pattern: Nur eine Instanz erlauben
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional, falls der Manager durch Szenenwechsel erhalten bleiben soll
    }

    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                SignInAnonymously();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Signed in anonymously as: " + user.UserId);

            // ✅ Jetzt schreiben wir eine Test-Nachricht in die DB
            WriteTestMessage();
        });
    }

    void WriteTestMessage()
    {
        string messageId = databaseReference.Child("messages").Push().Key;

        Dictionary<string, object> messageData = new Dictionary<string, object>
        {
            { "text", "Hallo von Unity!" },
            { "timestamp", ServerValue.Timestamp },
            { "uid", auth.CurrentUser.UserId }
        };

        databaseReference.Child("messages").Child(messageId).SetValueAsync(messageData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Nachricht erfolgreich geschrieben!");
            }
            else
            {
                Debug.LogError("Fehler beim Schreiben: " + task.Exception);
            }
        });
    }
}
