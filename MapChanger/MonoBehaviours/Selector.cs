using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// A MapObject with a reticle for selecting MapObjects on the world map.
    /// </summary>
    public abstract class Selector : MapObject, IPeriodicUpdater
    {
        protected const float MAP_FRONT_Z = -30f;
        protected const float DEFAULT_SIZE = 0.3f;
        protected const float DEFAULT_SELECTION_RADIUS = 0.7f;
        protected static readonly Vector4 DEFAULT_COLOR = new(1f, 1f, 1f, 0.6f);
        protected const string SELECTOR_SPRITE = "GUI.Selector";

        public ReadOnlyDictionary<string, ISelectable> Objects { get; private set; }

        public virtual Vector2 TargetPosition { get; } = Vector2.zero;
        public virtual float UpdateWaitSeconds { get; } = 0.02f;
        public virtual float SelectionRadius { get; } = DEFAULT_SELECTION_RADIUS;
        public virtual float SpriteSize { get; } = DEFAULT_SIZE;

        protected GameObject SpriteObject { get; private set; }
        protected SpriteRenderer Sr { get; private set; }

        private ISelectable _selectedObject;

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

        /// <summary>
        /// If LockSelection is on, the player can pan away from the selected object but maintain
        /// its selection. Resets on MainUpdate.
        /// </summary>
        private bool lockSelection = false;
        public bool LockSelection
        {
            get => lockSelection;
            set
            {
                if (lockSelection != value)
                {
                    if (value && _selectedObject is not null)
                    {
                        lockSelection = true;
                        StopPeriodicUpdate();
                    }
                    else
                    {
                        lockSelection = false;
                        StartPeriodicUpdate();
                    }
                }
            }
        }

        public void ToggleLockSelection()
        {
            LockSelection = !LockSelection;
        }

        private Coroutine periodicUpdate;
        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                double minDistance = SelectionRadius;
                ISelectable closestObj = null;

                foreach (ISelectable selectable in Objects.Values)
                {
                    if (!selectable.CanSelect()) continue;
                    
                    double distanceX = Math.Abs(selectable.Position.x - TargetPosition.x);
                    if (distanceX > minDistance) continue;

                    double distanceY = Math.Abs(selectable.Position.y - TargetPosition.y);
                    if (distanceY > minDistance) continue;

                    double euclidDistance = Math.Pow(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2), 0.5f);

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
            if (periodicUpdate is null)
            {
                periodicUpdate = StartCoroutine(PeriodicUpdate());
            }
        }

        private void StopPeriodicUpdate()
        {
            if (periodicUpdate is not null)
            {
                StopCoroutine(periodicUpdate);
                periodicUpdate = null;
            }
        }

        public virtual void Initialize(IEnumerable<ISelectable> objects)
        {
            base.Initialize();

            Objects = new(objects.ToDictionary(o => o.Key, o => o));

            DontDestroyOnLoad(this);

            ActiveModifiers.Add(WorldMapOpen);

            SpriteObject = new("Selector Sprite");
            SpriteObject.transform.SetParent(transform, false);
            SpriteObject.layer = UI_LAYER;

            Sr = SpriteObject.AddComponent<SpriteRenderer>();
            Sr.sprite = SpriteManager.Instance.GetSprite(SELECTOR_SPRITE);
            Sr.color = DEFAULT_COLOR;
            Sr.sortingLayerName = HUD;

            transform.localScale = Vector3.one * SpriteSize;
            transform.localPosition = new Vector3(0, 0, MAP_FRONT_Z);

            MapObjectUpdater.Add(this);
        }

        private bool WorldMapOpen()
        {
            return States.WorldMapOpen;
        }

        public override void OnMainUpdate(bool active)
        {
            lockSelection = false;

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
}
