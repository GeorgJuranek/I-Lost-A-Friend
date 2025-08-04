using UnityEngine;

public interface INestable
{
    public abstract Transform TemporaryParent { get; set; }

    public abstract void CheckForNest(Transform parentalTrigger);

    public abstract void Nest(Transform temporaryParent);

    public abstract void Unnest();
}
