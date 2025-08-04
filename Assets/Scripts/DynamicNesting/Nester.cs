using UnityEngine;

public class Nester : MonoBehaviour, INestable
{
    Transform highestHierachyLevelTransform; //at Start

    Transform temporaryParent = null;
    public Transform TemporaryParent { get => temporaryParent; set { temporaryParent = value; } }

    bool isNested;

    [SerializeField]
    FallDetector fallDetector;

    private void Awake()
    {
        highestHierachyLevelTransform = GetHighestParentRecursive(this.transform);
    }

    public void CheckForNest(Transform parenter)
    {
        if (temporaryParent == parenter) return;

        Debug.DrawLine(transform.position, transform.position + Vector3.down * 4f, Color.magenta);
        Debug.DrawLine(transform.position, parenter.position, Color.cyan, 1f);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastFind, 4f) ||
            Physics.Raycast(transform.position, (parenter.position - transform.position).normalized, out raycastFind, 1f)) 
        {
            if (!isNested)
            {
                Nest(parenter);
            }
        }
    }

    public void Nest(Transform newParent)
    {
        if (temporaryParent == newParent) return;

        temporaryParent = newParent;
        highestHierachyLevelTransform.parent = newParent;
        isNested = true;
        fallDetector.StopFalling();
    }

    public void Unnest()
    {
        if (!isNested) return;

        temporaryParent = null;
        highestHierachyLevelTransform.parent = null;
        isNested = false;
        fallDetector.StartFalling();
    }

    Transform GetHighestParentRecursive(Transform obj)
    {
        if (obj.parent == null)
        {
            return obj;
        }
        return GetHighestParentRecursive(obj.parent.transform);
    }


}
