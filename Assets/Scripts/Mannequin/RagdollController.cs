using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    Nester nestable;

    Collider[] colliders;
    Rigidbody[] rigidbodies;
    Joint[] joints;

    private List<RagdollPart> ragdollParts = new List<RagdollPart>();

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        joints = GetComponentsInChildren<Joint>();
    }

    private void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            RagdollPart part = new RagdollPart
            {
                transform = child,
                localPosition = child.localPosition,
                localRotation = child.localRotation
            };
            ragdollParts.Add(part);
        }
    }

    public void SetColliders(bool isActive)
    {
        foreach (var collider in colliders)
        {
            collider.enabled = isActive;
        }
    }

    public void SetJoints(bool isActive)
    {
        foreach (var joint in joints)
        {
            joint.enableCollision = isActive;
            joint.enablePreprocessing = isActive;
        }
    }

    public void SetRigidbodiesKinematic(bool isActive)
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = isActive;
        }
    }

    public void SetAllCollision(bool mode)
    {
        SetColliders(mode);
        SetJoints(mode);
    }

    public float getAverageVelocity()
    {
        float allVelocities = 0f;

        foreach(var rigidbody in rigidbodies)
        {
            allVelocities += rigidbody.velocity.magnitude;
        }

        return allVelocities / rigidbodies.Length;
    }

    public void TeleportRigidbodies(Vector3 newPosition)
    {

        SetRigidbodiesKinematic(true);
        SetColliders(false);

        nestable.Unnest();

        foreach (var part in ragdollParts)
        {
            part.transform.localPosition = part.localPosition;
            part.transform.localRotation = part.localRotation;
        }

        transform.position = newPosition;
        Physics.SyncTransforms();

        StartCoroutine(CompleteResetWithJointAndInertia());
    }

    private IEnumerator CompleteResetWithJointAndInertia()
    {
        yield return new WaitForFixedUpdate();

        ResetPhysics();
        ResetInertiaAndJoints();

        yield return new WaitForFixedUpdate();

        SetRigidbodiesKinematic(false);
        SetColliders(true);
    }

    private void ResetPhysics()
    {
        foreach (var rigidbody in rigidbodies)
        {
            if (rigidbody.isKinematic) continue;

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void ResetInertiaAndJoints()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.ResetInertiaTensor(); 
        }

        foreach (var joint in joints)
        {
            if (joint is ConfigurableJoint configurableJoint)
            {
                configurableJoint.targetRotation = Quaternion.identity;
                configurableJoint.targetAngularVelocity = Vector3.zero;
            }
        }
    }

    public void SetVelocityForAllRigidbodies(Vector3 newVelocity)
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.velocity = newVelocity;
        }
    }

    public void SetRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

    private struct RagdollPart
    {
        public Transform transform;
        public Vector3 localPosition;
        public Quaternion localRotation;
    }


    void ResetRagdollPositionsRelative()
    {
        foreach (var part in ragdollParts)
        {
            part.transform.localPosition = part.localPosition;
            part.transform.localRotation = part.localRotation;
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
