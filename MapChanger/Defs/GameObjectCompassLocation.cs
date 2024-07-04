using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class GameObjectCompassLocation(GameObject go) : CompassLocation
    {
        public GameObject GameObject { get; } = go;
        public override Vector2 Position => GameObject.transform.position;
    }
}