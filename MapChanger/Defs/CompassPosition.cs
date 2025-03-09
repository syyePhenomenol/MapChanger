using Modding.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapChanger.Defs
{
    public abstract class CompassPosition
    {
        public Vector2? Value { get; protected set; }
        public abstract void UpdateEveryFrame();
        public abstract void UpdateInfrequent();
    }

    public class FixedCompassPosition : CompassPosition
    {
        public FixedCompassPosition(Vector2? position)
        {
            Value = position;
        }

        public FixedCompassPosition((float x, float y) coords)
        {
            Value = new(coords.x, coords.y);
        }

        public override sealed void UpdateEveryFrame() { }
        public override sealed void UpdateInfrequent() { }
    }

    public class GameObjectCompassPosition(string goPath, bool updateEveryFrame) : CompassPosition
    {
        protected string GoPath => goPath;

        private GameObject _go;

        public override void UpdateEveryFrame()
        {
            if (updateEveryFrame)
            {
                Update();
            }
        }

        public override void UpdateInfrequent()
        {
            if (!updateEveryFrame)
            {
                Update();
            }
        }

        private void Update()
        {
            if (_go == null && TryGetGameObject(out GameObject go))
            {
                _go = go;
            }

            Value = _go != null && _go.activeInHierarchy ? _go.transform.position
                : Value is not null ? Value
                : null;
        }

        public virtual bool TryGetGameObject(out GameObject goResult)
        {
            Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            try
            {
                if (UnityExtensions.FindGameObject(activeScene, goPath) is GameObject go)
                {
                    goResult = go;
                    return true;
                }
            }
            catch { }

            goResult = null;
            return false;
        }
    }
}
