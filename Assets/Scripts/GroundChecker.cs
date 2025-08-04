using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    List<string> layerNames;

    LayerMask legitLayerMask;

    private void Start()
    {
        foreach (string layerName in layerNames)
        {
            int layer = LayerMask.NameToLayer(layerName); //translate strings
            if (layer != -1) //does layer exist
            {
                legitLayerMask |= (1 << layer); //bitshifts to layermask position and can combine different layermask in one
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist!");
            }
        }
    }

    //With layermasks
    public bool GroundCheck()
    {
        bool isInGroundContact = Physics.Raycast(transform.position, Vector3.down, 0.5f, legitLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red, 0.5f);

        return isInGroundContact;
    }


    public bool GroundCheck(float length)
    {
        bool isInGroundContact = Physics.Raycast(transform.position, Vector3.down, length, legitLayerMask);
        Debug.DrawRay(transform.position, Vector3.down * length, Color.red, length);

        return isInGroundContact;
    }

    public bool GroundCheckFromOtherGameObjectSphereCollider(GameObject groundChecker)
    {
        Vector3 checkerPosition = groundChecker.transform.position;
        float checkerRadius = groundChecker.GetComponent<SphereCollider>().radius;
        bool isInGroundContact = Physics.CheckSphere(checkerPosition, checkerRadius, legitLayerMask);

        return isInGroundContact;
    }

    public bool GroundCheckFromOtherGameObjectBoxCollider(GameObject groundChecker)
    {
        BoxCollider box = groundChecker.GetComponent<BoxCollider>();
        if (box == null)
        {
            Debug.LogWarning("No BoxCollider found on the groundChecker GameObject.");
            return false;
        }

        Vector3 boxCenter = box.transform.position + box.center;
        Vector3 boxSize = box.size * 0.5f;
        Quaternion boxRotation = box.transform.rotation;

        bool isInGroundContact = Physics.CheckBox(boxCenter, boxSize, boxRotation, legitLayerMask);

        return isInGroundContact;
    }



    public bool GroundCheckFromOtherGameObjectCapsuleCollider(GameObject groundChecker)
    {
        CapsuleCollider capsule = groundChecker.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            Debug.LogWarning("No CapsuleCollider found on the groundChecker GameObject.");
            return false;
        }

        Vector3 point1 = capsule.transform.position + capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius);
        Vector3 point2 = capsule.transform.position + capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius);
        float radius = capsule.radius;

        Debug.DrawLine(point1, point2, Color.red);
        Debug.DrawLine(point1 + Vector3.right * radius, point2 + Vector3.right * radius, Color.green);
        Debug.DrawLine(point1 - Vector3.right * radius, point2 - Vector3.right * radius, Color.green);

        bool isInGroundContact = Physics.CheckCapsule(point1, point2, radius, legitLayerMask);

        return isInGroundContact;
    }




    // Without Layermasks
    public bool GroundCheckAny()
    {
        bool isInGroundContact = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        Debug.DrawRay(transform.position, Vector3.down * 0.5f, Color.red, 0.5f);

        return isInGroundContact;
    }

    public bool GroundCheckAny(float length)
    {
        bool isInGroundContact = Physics.Raycast(transform.position, Vector3.down, length);
        Debug.DrawRay(transform.position, Vector3.down * length, Color.red, length);

        return isInGroundContact;
    }

    // Specific irregular Layer
    public bool GroundCheckSpecific(string layerName, float length)
    {
        bool isInGroundContact = Physics.Raycast(transform.position, Vector3.down, length, LayerMask.NameToLayer(layerName));
        Debug.DrawRay(transform.position, Vector3.down * length, Color.red, 0.5f);

        return isInGroundContact;
    }
}
