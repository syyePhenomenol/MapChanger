using System.Collections.Generic;
using MapChanger.Input;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

internal class InputListener : MonoBehaviour
{
    private IEnumerable<BindableInput> _inputs = [];

    public void Initialize(IEnumerable<BindableInput> inputs)
    {
        DontDestroyOnLoad(this);
        _inputs = inputs;
    }

    public void Update()
    {
        foreach (var input in _inputs)
        {
            input.Update();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
