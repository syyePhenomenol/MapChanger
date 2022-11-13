using System;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class NextAreaArrow : ColoredMapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private SpriteRenderer sr;

        public override Vector4 Color
        {
            get => sr.color;
            set
            {
                sr.color = value;
            }
        }

        private MapNextAreaDisplay Mnad => transform.parent.GetComponent<MapNextAreaDisplay>();

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            MiscObjectDef = miscObjectDef;

            ActiveModifiers.Add(QuickMapOpen);
            ActiveModifiers.Add(IsActive);

            sr = transform?.GetComponent<SpriteRenderer>();

            if (sr == null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component reference! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = sr.color;

            MapObjectUpdater.Add(this);
        }

        private bool IsActive()
        {
            if (Settings.MapModEnabled())
            {
                try { return Settings.CurrentMode().NextAreaArrowActiveOverride(this) ?? DefaultActive(); }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }

            return DefaultActive();

            bool DefaultActive()
            {
                return Mnad.visitedString is "" || PlayerData.instance.GetBool(Mnad.visitedString);
            }
        }

        private bool QuickMapOpen()
        {
            return States.QuickMapOpen;
        }

        public override void UpdateColor()
        {
            if (Settings.MapModEnabled())
            {
                try { Color = Settings.CurrentMode().NextAreaColorOverride(MiscObjectDef) ?? OrigColor; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }
            else
            {
                ResetColor();
            }
        }
    }
}
