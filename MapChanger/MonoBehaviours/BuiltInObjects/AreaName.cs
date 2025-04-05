using System;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class AreaName : ColoredMapObject
{
    private TextMeshPro _tmp;

    public MiscObjectDef MiscObjectDef { get; private set; }
    public override Vector4 Color
    {
        get => _tmp.color;
        set => _tmp.color = value;
    }

    internal void Initialize(MiscObjectDef miscObjectDef)
    {
        ActiveModifiers.Add(AreaNamesEnabled);

        MiscObjectDef = miscObjectDef;

        _tmp = GetComponent<TextMeshPro>();

        if (_tmp == null)
        {
            MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
            Destroy(this);
            return;
        }

        OrigColor = _tmp.color;

        MapObjectUpdater.Add(this);
    }

    private bool AreaNamesEnabled()
    {
        return !(Settings.MapModEnabled() && Settings.CurrentMode().DisableAreaNames);
    }

    public override void UpdateColor()
    {
        if (Settings.MapModEnabled())
        {
            try
            {
                Color = Settings.CurrentMode().AreaNameColorOverride(this) ?? OrigColor;
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
