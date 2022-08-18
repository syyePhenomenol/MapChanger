using System;
using GlobalEnums;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class QuickMapTitle : ColoredMapObject
    {
        public static QuickMapTitle Instance { get; private set; }

        private TextMeshPro tmp;
        public override Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
            }
        }

        public override void Initialize()
        {
            Instance = this;

            tmp = GetComponent<TextMeshPro>();
            OrigColor = tmp.color;

            gameObject.SetActive(true);

            MapObjectUpdater.Add(this);
        }

        public override void UpdateColor()
        {
            if (Settings.MapModEnabled())
            {
                try { Color = Settings.CurrentMode().QuickMapTitleColorOverride(this) ?? OrigColor; }
                catch (Exception e) { MapChangerMod.Instance.LogError(e); }
            }
            else
            {
                ResetColor();
            }
        }
    }
}
