using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class Pin : MapObject
    {
        private bool snapPosition = true;
        public bool SnapPosition
        {
            get => snapPosition;
            set
            {
                if (snapPosition != value)
                {
                    snapPosition = value;
                    UpdatePosition();
                }
            }
        }

        private IMapPosition mapPosition;
        public IMapPosition MapPosition
        {
            get => mapPosition;
            set
            {
                if (mapPosition != value)
                {
                    mapPosition = value;
                    UpdatePosition();
                }
            }
        }

        protected SpriteRenderer Sr { get; private set; }
        public Sprite Sprite
        {
            get => Sr.sprite;
            set
            {
                Sr.sprite = value;
            }
        }

        public Vector4 Color
        {
            get => Sr.color;
            set
            {
                Sr.color = value;
            }
        }

        private float size = 1f;
        public float Size
        {
            get => size;
            set
            {
                size = value;
                transform.localScale = new(size, size, transform.localScale.z);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            GameObject goPinSprite = new($"{transform.name} Pin Sprite");

            goPinSprite.layer = UI_LAYER;
            goPinSprite.transform.SetParent(transform, false);

            Sr = goPinSprite.AddComponent<SpriteRenderer>();
            Sr.sortingLayerName = HUD;
        }

        private void UpdatePosition()
        {
            if (mapPosition is null) return;

            if (snapPosition)
            {
                transform.localPosition = new Vector3(mapPosition.X.Snap(), mapPosition.Y.Snap(), transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(mapPosition.X, mapPosition.Y, transform.localPosition.z);
            }
        }
    }
}
