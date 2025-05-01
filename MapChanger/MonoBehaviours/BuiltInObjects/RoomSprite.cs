using System;
using System.Collections.Generic;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public class RoomSprite : ColoredMapObject, ISelectable
{
    private SpriteRenderer _sr;

    private bool _selected;

    public RoomSpriteDef Rsd { get; private set; }

    public override Vector4 Color
    {
        get => _sr.color;
        set => _sr.color = value;
    }

    public bool Selected
    {
        get => _selected;
        set
        {
            if (_selected != value)
            {
                _selected = value;
                UpdateColor();
            }
        }
    }

    public string Key => Rsd.SceneName;
    public Vector2 Position => transform.position;

    internal Vector2 Dimensions { get; private set; }

    internal void Initialize(RoomSpriteDef rsd)
    {
        Rsd = rsd;

        ActiveModifiers.Add(IsActive);

        // Hotfix for AdditionalMaps with different GameObject hierarchy
        if (transform.parent.name is "WHITE_PALACE" or "GODS_GLORY")
        {
            _sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        else
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
        }

        Dimensions = _sr.sprite.bounds.size;

        OrigColor = _sr.color;

        if (!Finder.IsScene(Rsd.SceneName))
        {
            MapChangerMod.Instance.LogDebug($"Not a scene: {Rsd.SceneName}");
        }

        MapObjectUpdater.Add(this);
    }

    private bool IsActive()
    {
        if (Settings.MapModEnabled())
        {
            try
            {
                return Settings.CurrentMode().RoomActiveOverride(this) ?? DefaultActive();
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }

        return DefaultActive();

        bool DefaultActive()
        {
            return (Settings.MapModEnabled() && Settings.CurrentMode().FullMap)
                || PlayerData.instance.GetVariable<List<string>>("scenesMapped").Contains(Rsd.SceneName)
                || Finder.IsMinimalMapScene(transform.name);
        }
    }

    public override void UpdateColor()
    {
        if (Settings.MapModEnabled())
        {
            try
            {
                Color = Settings.CurrentMode().RoomColorOverride(this) ?? OrigColor;
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

    public bool CanSelect()
    {
        if (Settings.MapModEnabled())
        {
            try
            {
                return Settings.CurrentMode().RoomCanSelectOverride(this) ?? gameObject.activeInHierarchy;
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }

        return gameObject.activeInHierarchy;
    }
}
