using UnityEngine;

public class FootAlignment : MonoBehaviour
{
    [SerializeField]
    float rayLength = 0.1f;

    [SerializeField]
    LayerMask groundLayer;

    private void Update()
    {
        AlignFootToSurface();
    }

    private void AlignFootToSurface()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Vector3 euler = targetRotation.eulerAngles;

            euler.y = transform.rotation.eulerAngles.y;

            transform.rotation = Quaternion.Euler(euler);
        }
        else
        {
            Vector3 euler = transform.localRotation.eulerAngles;
            euler.x = 0f;
            euler.z = 0f;
            transform.localRotation = Quaternion.Euler(euler);
        }

        // DEBUG
        Debug.DrawRay(transform.position, Vector3.down * rayLength, hit.collider ? Color.green : Color.red);
    }
}
