using System.Collections.Generic;
using UnityEngine;

public class Parenter : MonoBehaviour
{
    List<INestable> nestedNestables;

    private void Awake()
    {
        nestedNestables = new List<INestable>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<INestable>(out INestable nestable))
        {
            nestable.CheckForNest(this.transform);

            if (!nestedNestables.Contains(nestable))
                nestedNestables.Add(nestable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<INestable>(out INestable nestable))
        {
            nestable.Unnest();

            if (nestedNestables.Contains(nestable))
                nestedNestables.Remove(nestable);
        }
    }


    public void ForceNestSpecific(GameObject toNest)
    {
        if (toNest.TryGetComponent<INestable>(out INestable nestable))
        {
            nestable.Nest(this.transform);
        }
    }

    public void ForceUnestAll()
    {
        if (nestedNestables.Count == 0) return;

        foreach (var nested in nestedNestables)
        {
            nested.Unnest();
        }

    }
}
