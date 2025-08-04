using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class DynamicText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textField;

    [SerializeField]
    RectTransform contentRect;

    [SerializeField]
    ScrollRect scrollRect;

    private void OnEnable()
    {
        MessageManager.OnFetchWasSuccessful += AdjustContentSize;
    }

    private void OnDisable()
    {
        MessageManager.OnFetchWasSuccessful -= AdjustContentSize;
    }

    void AdjustContentSize()
    {
        Canvas.ForceUpdateCanvases();
        float newHeight = textField.preferredHeight+100f;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
        scrollRect.verticalNormalizedPosition = 1f;

        scrollRect.verticalScrollbar.value = 0f; //Sets in this case to bottom of scroll
    }
}
