using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Saver))]

public class SavePoint : MonoBehaviour
{
    bool isActivated;

    Mover mover;
    Saver saver;

    public static Action OnPlayerWasSaved;

    private void Start()
    {
        mover = GetComponent<Mover>();
        saver = GetComponent<Saver>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            saver.TryToSaveFromCollision(other);

            OnPlayerWasSaved?.Invoke();

            mover.Move(Mover.ETargets.Up, 10f);
        }
    }
}

