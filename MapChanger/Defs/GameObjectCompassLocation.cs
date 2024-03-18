using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class GameObjectCompassLocation : CompassLocation
    {
        private readonly GameObject go;
        public override Vector2 Position => go.transform.position;
        public GameObjectCompassLocation(GameObject go)
        {
            this.go = go;
        }
    }
}