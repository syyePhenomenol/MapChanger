using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    /// <summary>
    /// A MapObject with a reticle for selecting MapObjects on the world map.
    /// </summary>
    public abstract class Selector : MapObject, IPeriodicUpdater
    {
        public const string NONE_SELECTED = "None selected";

        protected const float MAP_FRONT_Z = -30f;
        protected const float DEFAULT_SIZE = 0.3f;
        protected const float DEFAULT_SELECTION_RADIUS = 0.7f;
        protected static readonly Vector4 DEFAULT_COLOR = new(1f, 1f, 1f, 0.6f);
        protected const string SELECTOR_SPRITE = "GUI.Selector";

        public Dictionary<string, List<ISelectable>> Objects { get; } = new();
        public virtual Vector2 TargetPosition { get; } = Vector2.zero;
        public virtual float UpdateWaitSeconds { get; } = 0.02f;
        public virtual float SelectionRadius { get; } = DEFAULT_SELECTION_RADIUS;
        public virtual float SpriteSize { get; } = DEFAULT_SIZE;

        protected GameObject SpriteObject { get; private set; }
        protected SpriteRenderer Sr { get; private set; }

        private string selectedObjectKey = NONE_SELECTED;
        public string SelectedObjectKey
        {
            get => selectedObjectKey;
            private set
            {
                if (selectedObjectKey != value)
                {
                    if (selectedObjectKey is not NONE_SELECTED)
                    {
                        DeselectInternal(selectedObjectKey);
                    }

                    if (value is not NONE_SELECTED)
                    {
                        SelectInternal(value);
                    }

                    selectedObjectKey = value;
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
                    if (value && selectedObjectKey is not NONE_SELECTED)
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

        private void SelectInternal(string objectKey)
        {
            if (Objects.ContainsKey(objectKey))
            {
                foreach (ISelectable selectable in Objects[objectKey])
                {
                    Select(selectable);
                }
            }
        }

        private void DeselectInternal(string objectKey)
        {
            if (Objects.ContainsKey(objectKey))
            {
                foreach (ISelectable selectable in Objects[objectKey])
                {
                    Deselect(selectable);
                }
            }
        }

        private Coroutine periodicUpdate;
        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);

                double minDistance = SelectionRadius;
                string newKey = NONE_SELECTED;

                foreach (List<ISelectable> selectables in Objects.Values)
                {
                    foreach (ISelectable selectable in selectables)
                    {
                        if (!selectable.CanSelect()) continue;

                        (string key, Vector2 position) = selectable.GetKeyAndPosition();

                        double distanceX = Math.Abs(position.x - TargetPosition.x);
                        if (distanceX > minDistance) continue;

                        double distanceY = Math.Abs(position.y - TargetPosition.y);
                        if (distanceY > minDistance) continue;

                        double euclidDistance = Math.Pow(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2), 0.5f);

                        if (euclidDistance < minDistance)
                        {
                            newKey = key;
                            minDistance = euclidDistance;
                        }
                    }
                }

                SelectedObjectKey = newKey;
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

        public override void Initialize()
        {
            base.Initialize();

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
                SelectedObjectKey = NONE_SELECTED;
            }
        }

        protected abstract void Select(ISelectable selectable);

        protected abstract void Deselect(ISelectable selectable);

        protected virtual void OnSelectionChanged() { }
    }
}
