using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class BorderedBackgroundPin : BorderedPin
{
    private SpriteRenderer _backgroundSr;

    public Sprite BackgroundSprite
    {
        get => _backgroundSr.sprite;
        set => _backgroundSr.sprite = value;
    }

    public override void Initialize()
    {
        base.Initialize();

        BorderPlacement = BorderPlacement.InFront;

        GameObject goBackground = new($"{transform.name} Background") { layer = UI_LAYER };
        goBackground.transform.SetParent(transform, false);

        _backgroundSr = goBackground.AddComponent<SpriteRenderer>();
        _backgroundSr.sortingLayerName = HUD;
        _backgroundSr.transform.localPosition = new Vector3(0f, 0f, 0.00001f);
    }
}
