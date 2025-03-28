using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class CompassTarget
    {
        public CompassPosition Position { get; init; }

        public abstract Color GetColor();

        public abstract Vector3 GetScale();

        public abstract Sprite GetSprite();

        public abstract bool IsActive();
    }
}