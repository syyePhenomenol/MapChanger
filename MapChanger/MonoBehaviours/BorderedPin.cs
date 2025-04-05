using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public enum BorderPlacement
{
    Behind,
    InFront,
}

public class BorderedPin : Pin
{
    private static readonly Dictionary<BorderPlacement, float> _borderOffset =
        new() { { BorderPlacement.Behind, 0.00001f }, { BorderPlacement.InFront, -0.00001f } };

    private SpriteRenderer _borderSr;
    private BorderPlacement _borderPlacement = BorderPlacement.InFront;

    public Sprite BorderSprite
    {
        get => _borderSr.sprite;
        set => _borderSr.sprite = value;
    }

    public Vector4 BorderColor
    {
        get => _borderSr.color;
        set => _borderSr.color = value;
    }
    public BorderPlacement BorderPlacement
    {
        get => _borderPlacement;
        set
        {
            if (_borderPlacement != value)
            {
                _borderPlacement = value;
                UpdateBorder();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        GameObject goBorder = new($"{transform.name} Border") { layer = UI_LAYER };
        goBorder.transform.SetParent(transform, false);

        _borderSr = goBorder.AddComponent<SpriteRenderer>();
        _borderSr.sortingLayerName = HUD;
        UpdateBorder();
    }

    private void UpdateBorder()
    {
        _borderSr.transform.localPosition = new Vector3(0f, 0f, _borderOffset[_borderPlacement]);
    }
}
