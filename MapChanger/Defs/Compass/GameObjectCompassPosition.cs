using Modding.Utils;
using UnityEngine;

namespace MapChanger.Defs;

public class GameObjectCompassPosition(string goPath, bool updateEveryFrame) : CompassPosition
{
    private GameObject _go;

    protected string GoPath => goPath;

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
        if (_go == null && TryGetGameObject(out var go))
        {
            _go = go;
        }

        Value = _go != null && _go.activeInHierarchy ? _go.transform.position : Value;
    }

    public virtual bool TryGetGameObject(out GameObject goResult)
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

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
