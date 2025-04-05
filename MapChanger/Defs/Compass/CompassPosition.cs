using UnityEngine;

namespace MapChanger.Defs;

public abstract class CompassPosition
{
    public Vector2? Value { get; protected set; }

    public abstract void UpdateEveryFrame();
    public abstract void UpdateInfrequent();
}
