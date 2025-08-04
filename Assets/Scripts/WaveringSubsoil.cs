using UnityEngine;

public class WaveringSubsoil : MonoBehaviour
{
    [SerializeField]
    Vector3 direction = Vector3.up;

    [SerializeField]
    float waveringIntensity = 1.2f;

    [SerializeField]
    float maxRotation = 45;

    Vector3 startRotation;

    private void Awake()
    {
        startRotation = transform.localEulerAngles;
    }

    void Update()
    {
        Vector3 newRotation = (direction * waveringIntensity) * (Mathf.Sin(Time.time));
        Vector3 clampedRotation = new Vector3( Mathf.Clamp(newRotation.x, -maxRotation, maxRotation), Mathf.Clamp(newRotation.y, -maxRotation, maxRotation), Mathf.Clamp(newRotation.z, -maxRotation, maxRotation));

        transform.localEulerAngles = startRotation+newRotation;
    }

}
