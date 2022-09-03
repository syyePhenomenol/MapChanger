using System;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class AreaName : ColoredMapObject
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

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            ActiveModifiers.Add(AreaNamesEnabled);

            MiscObjectDef = miscObjectDef;

            tmp = GetComponent<TextMeshPro>();

            if (tmp == null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = tmp.color;

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
                try { Color = Settings.CurrentMode().AreaNameColorOverride(this) ?? OrigColor; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }
            else
            {
                ResetColor();
            }
        }
    }
}
