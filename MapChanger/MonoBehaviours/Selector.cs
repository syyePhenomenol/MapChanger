using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

/// <summary>
/// A MapObject with a reticle for selecting MapObjects on the world map.
/// </summary>
public abstract class Selector : MapInputListener, IPeriodicUpdater
{
    protected const float MAP_FRONT_Z = -30f;
    protected const float DEFAULT_SIZE = 0.3f;
    protected const float DEFAULT_SELECTION_RADIUS = 0.7f;
    protected const string SELECTOR_SPRITE = "GUI.Selector";

    protected static readonly Vector4 DEFAULT_COLOR = new(1f, 1f, 1f, 0.6f);

    private ISelectable _selectedObject;

    /// <summary>
    /// If LockSelection is on, the player can pan away from the selected object but maintain
    /// its selection. Resets on MainUpdate.
    /// </summary>
    private bool _lockSelection = false;

    private Coroutine _periodicUpdate;

    public ReadOnlyDictionary<string, ISelectable> Objects { get; private set; }

    public virtual Vector2 TargetPosition { get; } = Vector2.zero;
    public virtual float UpdateWaitSeconds { get; } = 0.02f;
    public virtual float SelectionRadius { get; } = DEFAULT_SELECTION_RADIUS;
    public virtual float SpriteSize { get; } = DEFAULT_SIZE;

    protected GameObject SpriteObject { get; private set; }
    protected SpriteRenderer Sr { get; private set; }

    public ISelectable SelectedObject
    {
        get => _selectedObject;
        private set
        {
            if (_selectedObject != value)
            {
                if (_selectedObject is not null)
                {
                    Deselect(_selectedObject);
                }

                if (value is not null)
                {
                    Select(value);
                }

                _selectedObject = value;
                OnSelectionChanged();
            }
        }
    }

    public bool LockSelection
    {
        get => _lockSelection;
        set
        {
            if (_lockSelection != value)
            {
                if (value && _selectedObject is not null)
                {
                    _lockSelection = true;
                    StopPeriodicUpdate();
                }
                else
                {
                    _lockSelection = false;
                    StartPeriodicUpdate();
                }
            }
        }
    }

    public void ToggleLockSelection()
    {
        LockSelection = !LockSelection;
    }

    public IEnumerator PeriodicUpdate()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

            double minDistance = SelectionRadius;
            ISelectable closestObj = null;

            foreach (var selectable in Objects.Values)
            {
                if (!selectable.CanSelect())
                    continue;

                double distanceX = Math.Abs(selectable.Position.x - TargetPosition.x);
                if (distanceX > minDistance)
                    continue;

                double distanceY = Math.Abs(selectable.Position.y - TargetPosition.y);
                if (distanceY > minDistance)
                    continue;

                var euclidDistance = Math.Pow(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2), 0.5f);

                if (euclidDistance < minDistance)
                {
                    closestObj = selectable;
                    minDistance = euclidDistance;
                }
            }

            SelectedObject = closestObj;
        }
    }

    private void StartPeriodicUpdate()
    {
        _periodicUpdate ??= StartCoroutine(PeriodicUpdate());
    }

    private void StopPeriodicUpdate()
    {
        if (_periodicUpdate is not null)
        {
            StopCoroutine(_periodicUpdate);
            _periodicUpdate = null;
        }
    }

    public virtual void Initialize(IEnumerable<MapInput> inputs, IEnumerable<ISelectable> objects)
    {
        Initialize(inputs);

        Objects = new(objects.ToDictionary(o => o.Key, o => o));

        SpriteObject = new("Selector Sprite");
        SpriteObject.transform.SetParent(transform, false);
        SpriteObject.layer = UI_LAYER;

        Sr = SpriteObject.AddComponent<SpriteRenderer>();
        Sr.sprite = SpriteManager.Instance.GetSprite(SELECTOR_SPRITE);
        Sr.color = DEFAULT_COLOR;
        Sr.sortingLayerName = HUD;

        transform.localScale = Vector3.one * SpriteSize;
        transform.localPosition = new Vector3(0, 0, MAP_FRONT_Z);
    }

    public override void OnMainUpdate(bool active)
    {
        _lockSelection = false;

        if (active)
        {
            StartPeriodicUpdate();
        }
        else
        {
            StopPeriodicUpdate();
            SelectedObject = null;
        }
    }

    protected virtual void Select(ISelectable selectable)
    {
        selectable.Selected = true;
    }

    protected virtual void Deselect(ISelectable selectable)
    {
        selectable.Selected = false;
    }

    protected virtual void OnSelectionChanged() { }
}
