using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blinkerer : MonoBehaviour
{
    public Volume postProcessingVolume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    [SerializeField]
    float blinkingSpeed = 3f;

    [SerializeField]
    float startValueColorAdjustment, targetValueColorAdjustmentFall, targetValueColorAdjustmentPain,
        startValueVignette, targetValueVignette;

    bool isInCoroutine;
    Coroutine currentCoroutine = null;

    void Awake()
    {
        if (postProcessingVolume != null)
        {
            if (!postProcessingVolume.profile.TryGet(out vignette))
            {
                Debug.LogWarning("No vignette was found for Blinkerer");
            }

            if (!postProcessingVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.LogWarning("No colorAdjustments was found for Blinkerer");
            }
        }
    }

    private void Start()
    {
        OnlyOpenEyes();
    }

    private void OnEnable()
    {
        PlayerReseter.OnPlayerReset += PlayBlinkingOnFall;
        PlayerDeathInitiator.OnPlayerPain += PlayBlinkingOnPain;

        BalloonIGrabable.OnPlayerHasReachedGoal += OnlyCloseEyes;
    }

    private void OnDisable()
    {
        PlayerReseter.OnPlayerReset -= PlayBlinkingOnFall;
        PlayerDeathInitiator.OnPlayerPain -= PlayBlinkingOnPain;

        BalloonIGrabable.OnPlayerHasReachedGoal -= OnlyCloseEyes;
    }

    void PlayBlinkingOnFall()
    {
        currentCoroutine = StartCoroutine(Blinking(Color.black, targetValueColorAdjustmentFall));
    }

    void PlayBlinkingOnPain()
    {
        currentCoroutine = StartCoroutine(Blinking(Color.red, targetValueColorAdjustmentPain));
    }

    IEnumerator Blinking(Color color, float targetValueColorAdjustment)
    {
        if (isInCoroutine) yield break;

        vignette.color.value = color;

        isInCoroutine = true;

        bool secondCoroutineHasFinished = false;

        while(!secondCoroutineHasFinished)
        {
            yield return StartCoroutine(CloseEye(targetValueColorAdjustment));


            yield return StartCoroutine(OpenEye());

            secondCoroutineHasFinished = true;
        }

        vignette.color.value = Color.black;

        isInCoroutine = false;
    }

    IEnumerator CloseEye(float targetValueColorAdjustment)
    {
        while (Mathf.Abs(vignette.intensity.value - targetValueVignette) > 0.01f || Mathf.Abs(colorAdjustments.postExposure.value - targetValueColorAdjustment) > 0.01f)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetValueVignette, Time.deltaTime * blinkingSpeed);
            vignette.smoothness.value = Mathf.Lerp(vignette.intensity.value, targetValueVignette, Time.deltaTime * blinkingSpeed);

            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, targetValueColorAdjustment, Time.deltaTime * blinkingSpeed);

            yield return null;
        }

        vignette.intensity.value = targetValueVignette;
        colorAdjustments.postExposure.value = targetValueColorAdjustment;
    }

    IEnumerator OpenEye()
    {

        while (Mathf.Abs(vignette.intensity.value - startValueVignette) > 0.01f || Mathf.Abs(colorAdjustments.postExposure.value - startValueColorAdjustment) > 0.01f)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, startValueVignette, Time.deltaTime * blinkingSpeed);
            vignette.smoothness.value = Mathf.Lerp(vignette.intensity.value, startValueVignette, Time.deltaTime * blinkingSpeed);

            colorAdjustments.postExposure.value = Mathf.Lerp(colorAdjustments.postExposure.value, startValueColorAdjustment, Time.deltaTime * blinkingSpeed);

            yield return null;
        }

        vignette.intensity.value = startValueVignette;
        colorAdjustments.postExposure.value = startValueColorAdjustment;

    }

    public void OnlyOpenEyes()
    {
        vignette.intensity.value = startValueVignette;
        colorAdjustments.postExposure.value = startValueColorAdjustment;

        currentCoroutine = StartCoroutine(OpenEye());
    }

    public void OnlyCloseEyes()
    {
        currentCoroutine = StartCoroutine(CloseEye(targetValueColorAdjustmentFall));

    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
