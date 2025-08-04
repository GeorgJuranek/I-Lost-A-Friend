using System.Collections.Generic;
using UnityEngine;

public class CollisionLogic : MonoBehaviour
{
    [SerializeField]
    private List<string> layerNames = new List<string>();

    public bool IsInCollision { get; private set; } = false;
    private LayerMask legitLayerMask;


    private void Start()
    {
        foreach (string layerName in layerNames)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1)
            {
                legitLayerMask |= (1 << layer);
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' does not exist!");
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & legitLayerMask) != 0)
        {
            IsInCollision = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & legitLayerMask) != 0)
        {
            IsInCollision = false;
        }
    }
}
