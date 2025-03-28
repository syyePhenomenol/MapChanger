﻿using System;
using System.Collections.Generic;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class RoomSprite : ColoredMapObject, ISelectable
    {
        public RoomSpriteDef Rsd { get; private set; }

        private SpriteRenderer sr;
        public override Vector4 Color
        {
            get => sr.color;
            set
            {
                sr.color = value;
            }
        }

        private bool selected = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    UpdateColor();
                }
            }
        }

        public string Key => Rsd.SceneName;
        public Vector2 Position => transform.position;

        internal void Initialize(RoomSpriteDef rsd)
        {
            Rsd = rsd;

            ActiveModifiers.Add(IsActive);
            
            // Hotfix for AdditionalMaps with different GameObject hierarchy
            if (transform.parent.name is "WHITE_PALACE" or "GODS_GLORY")
            {
                sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
            else
            {
                sr = GetComponentInChildren<SpriteRenderer>();
            }
            
            OrigColor = sr.color;

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
                try { return Settings.CurrentMode().RoomActiveOverride(this) ?? DefaultActive(); }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
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
                try { Color = Settings.CurrentMode().RoomColorOverride(this) ?? OrigColor; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
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
                try { return Settings.CurrentMode().RoomCanSelectOverride(this) ?? gameObject.activeInHierarchy; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }

            return gameObject.activeInHierarchy;
        }
    }
}
