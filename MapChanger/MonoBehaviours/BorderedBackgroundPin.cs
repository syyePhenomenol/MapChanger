using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class BorderedBackgroundPin : BorderedPin
    {
        private SpriteRenderer backgroundSr;
        public Sprite BackgroundSprite
        {
            get => backgroundSr.sprite;
            set
            {
                backgroundSr.sprite = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            BorderPlacement = BorderPlacement.InFront;

            GameObject goBackground = new($"{transform.name} Background")
            {
                layer = UI_LAYER
            };
            goBackground.transform.SetParent(transform, false);

            backgroundSr = goBackground.AddComponent<SpriteRenderer>();
            backgroundSr.sortingLayerName = HUD;
            backgroundSr.transform.localPosition = new Vector3(0f, 0f, 0.00001f);
        }
    }
}
