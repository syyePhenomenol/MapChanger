using System;
using MapChanger.Defs;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class NextArea : ColoredMapObject
    {
        public MiscObjectDef MiscObjectDef { get; private set; }

        private TextMeshPro tmp;
        private SpriteRenderer sr;
        public override Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
                sr.color = value;
            }
        }

        private MapNextAreaDisplay Mnad => GetComponent<MapNextAreaDisplay>();

        internal void Initialize(MiscObjectDef miscObjectDef)
        {
            MiscObjectDef = miscObjectDef;

            ActiveModifiers.Add(IsActive);

            tmp = transform.FindChildInHierarchy("Area Name")?.GetComponent<TextMeshPro>();
            sr = transform.FindChildInHierarchy("Map_Arrow")?.GetComponent<SpriteRenderer>();

            if (tmp is null || sr is null)
            {
                MapChangerMod.Instance.LogWarn($"Missing component references! {transform.name}");
                Destroy(this);
                return;
            }

            OrigColor = tmp.color;

            MapObjectUpdater.Add(this);
        }

        private bool IsActive()
        {
            return States.QuickMapOpen
                && !(Settings.MapModEnabled() && Settings.CurrentMode().DisableNextArea)
                && (Mnad.visitedString is "" || PlayerData.instance.GetBool(Mnad.visitedString));
        }

        public override void UpdateColor()
        {
            if (Settings.MapModEnabled())
            {
                try { Color = Settings.CurrentMode().NextAreaColorOverride(this) ?? OrigColor; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }
            else
            {
                ResetColor();
            }
        }
    }
}
