using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class CompassLocation
    {
        public virtual bool IsActive => true;
        public abstract Vector2 Position { get; }
        public abstract Sprite Sprite { get; }
        public abstract Color Color { get; }
    }
}