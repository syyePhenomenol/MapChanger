using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger;

public class MapMode
{
    public (string, string) ModeKey => (Mod, ModeName);
    public virtual string Mod => "MapChangerMod";
    public virtual string ModeName => "Disabled";

    /// <summary>
    /// If an instance of Settings is new, it will be initialized to a mode that
    /// has this return true. Ties are broken by Priority.
    /// </summary>
    public virtual bool InitializeToThis()
    {
        return false;
    }

    public virtual float Priority => float.PositiveInfinity;
    public virtual bool ForceHasMap => false;
    public virtual bool ForceHasQuill => false;
    public virtual bool? VanillaPins => null;
    public virtual bool? MapMarkers => null;

    /// <summary>
    /// Determines if the map immediately gets filled in when visiting a new scene with Quill.
    /// </summary>
    public virtual bool ImmediateMapUpdate => false;

    /// <summary>
    /// Forces all map areas/rooms to be visible and filled in.
    /// </summary>
    public virtual bool FullMap => false;

    /// <summary>
    /// Whether or not to display area names on the World Map or Quick Map.
    /// </summary>
    public virtual bool DisableAreaNames => false;

    public virtual bool? RoomActiveOverride(RoomSprite roomSprite)
    {
        return null;
    }

    public virtual bool? RoomCanSelectOverride(RoomSprite roomSprite)
    {
        return null;
    }

    public virtual Vector4? RoomColorOverride(RoomSprite roomSprite)
    {
        return null;
    }

    public virtual Vector4? AreaNameColorOverride(AreaName areaName)
    {
        return null;
    }

    public virtual bool? NextAreaNameActiveOverride(NextAreaName nextAreaName)
    {
        return null;
    }

    public virtual bool? NextAreaArrowActiveOverride(NextAreaArrow nextAreaArrow)
    {
        return null;
    }

    public virtual Vector4? NextAreaColorOverride(MiscObjectDef miscObjectDef)
    {
        return null;
    }

    public virtual Vector4? QuickMapTitleColorOverride(QuickMapTitle qmt)
    {
        return null;
    }
}
