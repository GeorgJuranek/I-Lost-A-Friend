using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabVisualizer : MonoBehaviour
{
    [SerializeField]
    Image openHand;

    [SerializeField]
    Holder playerHolder;

    [SerializeField]
    float disabledOpacity = 0.8f, enabledOpacity = 1f;


    private void OnEnable()
    {
        playerHolder.OnHoldableInRange += InRange;

        playerHolder.OnRightDistance += Highlight;

    }

    private void OnDisable()
    {
        playerHolder.OnHoldableInRange -= InRange;

        playerHolder.OnRightDistance -= Highlight;

    }

    private void Awake()
    {
        if (openHand == null || playerHolder == null)
        {
            return;
        }
        OutOfGrab();
    }

    void Highlight(bool isInRightDistance)
    {
        openHand.color = new Color(openHand.color.r, openHand.color.g, openHand.color.b, isInRightDistance ? enabledOpacity : disabledOpacity);
    }

    void InRange(bool isGrabableInRange)
    {
        openHand.enabled = isGrabableInRange;
    }

    void InGrab()
    {
        openHand.enabled = false;
    }

    void OutOfGrab()
    {
        openHand.enabled = false;
    }
}
