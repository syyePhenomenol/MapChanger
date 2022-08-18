using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public enum BorderPlacement
    {
        Behind,
        InFront
    }

    public class BorderedPin : Pin
    {
        private static readonly Dictionary<BorderPlacement, float> borderOffset = new()
        {
            { BorderPlacement.Behind, 0.00001f },
            { BorderPlacement.InFront, -0.00001f }
        };

        private SpriteRenderer borderSr;
        public Sprite BorderSprite
        {
            get => borderSr.sprite;
            set
            {
                borderSr.sprite = value;
            }
        }

        public Vector4 BorderColor
        {
            get => borderSr.color;
            set
            {
                borderSr.color = value;
            }
        }

        private BorderPlacement borderPlacement = BorderPlacement.InFront;
        public BorderPlacement BorderPlacement
        {
            get => borderPlacement;
            set
            {
                if (borderPlacement != value)
                {
                    borderPlacement = value;
                    UpdateBorder();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            GameObject goBorder = new($"{transform.name} Border");

            goBorder.layer = UI_LAYER;
            goBorder.transform.SetParent(transform, false);

            borderSr = goBorder.AddComponent<SpriteRenderer>();
            borderSr.sortingLayerName = HUD;
            UpdateBorder();
        }

        private void UpdateBorder()
        {
            borderSr.transform.localPosition = new Vector3(0f, 0f, borderOffset[borderPlacement]);
        }
    }
}
