using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

/// <summary>
/// Base class for all objects that appear directly on the map. If your derived object
/// is a root object, you probably want to initialize with DontDestroyOnLoad(), and to
/// manually destroy it at some point.
/// For UI elements that appear over the map, use MapUILayer instead.
/// </summary>
public class MapObject : MonoBehaviour
{
    /// <summary>
    /// A list of conditions to determine whether or not the GameObject should be set active or inactive.
    /// Short circuits on first false.
    /// </summary>
    public List<Func<bool>> ActiveModifiers { get; } = [];

    private protected const int UI_LAYER = 5;
    private protected const string HUD = "HUD";

    private readonly List<MapObject> _children = [];

    /// <summary>
    /// When MainUpdate is called on this MapObject, MainUpdate is also called on its children.
    /// Children are necessarily transform children on the parent MapObject.
    /// </summary>
    public ReadOnlyCollection<MapObject> Children => _children.AsReadOnly();

    public void AddChild(MapObject child)
    {
        if (_children.Contains(child))
            return;

        _children.Add(child);
        child.transform.parent = transform;
    }

    /// <summary>
    /// Use this method to do things right after the MapObject component is added to the GameObject.
    /// </summary>
    public virtual void Initialize()
    {
        gameObject.layer = UI_LAYER;

        ActiveModifiers.Add(MapChangerMod.IsEnabled);
    }

    /// <summary>
    /// The main method for updating the state of the MapObject. Also calls MainUpdate
    /// on its children.
    /// To have MainUpdate be called when the map opens or closes, add the MapObject to the
    /// MapObjectUpdater. If you want to update the MapObject at some other time (i.e. while
    /// the map is open), you need to handle this yourself.
    /// </summary>
    public void MainUpdate()
    {
        BeforeMainUpdate();

        var active = GetActive();

        gameObject.SetActive(active);

        OnMainUpdate(active);

        foreach (var mapObject in _children)
        {
            mapObject.MainUpdate();
        }

        bool GetActive()
        {
            foreach (var activeModifier in ActiveModifiers)
            {
                try
                {
                    if (!activeModifier.Invoke())
                        return false;
                }
                catch (Exception e)
                {
                    MapChangerMod.Instance.LogError(e);
                }
            }

            return true;
        }
    }

    /// <summary>
    /// User-defined behaviour before MainUpdate sets the active state of the MapObject.
    /// </summary>
    public virtual void BeforeMainUpdate() { }

    /// <summary>
    /// User-defined behaviour after MainUpdate sets the active state of the MapObject.
    /// </summary>
    /// <param name="active"></param>
    public virtual void OnMainUpdate(bool active) { }
}
