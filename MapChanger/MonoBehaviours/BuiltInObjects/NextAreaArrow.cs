using System;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class NextAreaArrow : ColoredMapObject
{
    private SpriteRenderer _sr;

    public MiscObjectDef MiscObjectDef { get; private set; }

    public override Vector4 Color
    {
        get => _sr.color;
        set => _sr.color = value;
    }

    private MapNextAreaDisplay Mnad => transform.parent.GetComponent<MapNextAreaDisplay>();

    internal void Initialize(MiscObjectDef miscObjectDef)
    {
        MiscObjectDef = miscObjectDef;

        ActiveModifiers.Add(QuickMapOpen);
        ActiveModifiers.Add(IsActive);

        _sr = transform?.GetComponent<SpriteRenderer>();

        if (_sr == null)
        {
            MapChangerMod.Instance.LogWarn($"Missing component reference! {transform.name}");
            Destroy(this);
            return;
        }

        OrigColor = _sr.color;

        MapObjectUpdater.Add(this);
    }

    private bool IsActive()
    {
        if (MapChangerMod.IsEnabled())
        {
            try
            {
                return ModeManager.CurrentMode().NextAreaArrowActiveOverride(this) ?? DefaultActive();
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }

        return DefaultActive();

        bool DefaultActive() => Mnad.visitedString is "" || PlayerData.instance.GetBool(Mnad.visitedString);
    }

    private bool QuickMapOpen()
    {
        return States.QuickMapOpen;
    }

    public override void UpdateColor()
    {
        if (MapChangerMod.IsEnabled())
        {
            try
            {
                Color = ModeManager.CurrentMode().NextAreaColorOverride(MiscObjectDef) ?? OrigColor;
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }
        else
        {
            ResetColor();
        }
    }
}
