using UnityEngine;

namespace MapChanger.Defs;

public class FixedCompassPosition : CompassPosition
{
    public FixedCompassPosition(Vector2? position)
    {
        Value = position;
    }

    public FixedCompassPosition((float x, float y) coords)
    {
        Value = new(coords.x, coords.y);
    }

    public sealed override void UpdateEveryFrame() { }

    public sealed override void UpdateInfrequent() { }
}
