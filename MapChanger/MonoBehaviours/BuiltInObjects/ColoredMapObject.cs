using UnityEngine;

namespace MapChanger.MonoBehaviours;

/// <summary>
/// Abstract derived class of MapObject for existing things on the map that have a color.
/// </summary>
public abstract class ColoredMapObject : MapObject
{
    public Vector4 OrigColor { get; private protected set; }
    public abstract Vector4 Color { get; set; }

    public override void OnMainUpdate(bool active)
    {
        UpdateColor();
    }

    public abstract void UpdateColor();

    public void ResetColor()
    {
        Color = OrigColor;
    }
}
