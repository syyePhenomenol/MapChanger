using System;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class NextAreaName : ColoredMapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private TextMeshPro tmp;

        public override Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
            }
        }

        private MapNextAreaDisplay Mnad => transform.parent.GetComponent<MapNextAreaDisplay>();

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            MiscObjectDef = miscObjectDef;

            ActiveModifiers.Add(QuickMapOpen);
            ActiveModifiers.Add(IsActive);

            tmp = transform?.GetComponent<TextMeshPro>();

            if (tmp == null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component reference! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = tmp.color;

            MapObjectUpdater.Add(this);
        }

        private bool IsActive()
        {
            if (Settings.MapModEnabled())
            {
                try { return Settings.CurrentMode().NextAreaNameActiveOverride(this) ?? DefaultActive(); }
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
