using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Joint))]

public class JointConnector : MonoBehaviour
{
    Joint joint;

    public bool HasConnection { get; set; }

    private void Start()
    {
        joint = GetComponent<Joint>();
    }

    public void Hold(GameObject toHold)
    {
        joint.connectedBody = toHold.GetComponent<Rigidbody>();
        HasConnection = true;
    }

    public void Unhold()
    {
        joint.connectedBody = null;
        HasConnection = false;
    }

}
