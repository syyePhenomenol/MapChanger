using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class CompassTarget
    {
        public CompassPosition Position { get; init; }

        public abstract bool IsActive();
        public abstract Sprite GetSprite();
        public abstract Color GetColor();
    }
}