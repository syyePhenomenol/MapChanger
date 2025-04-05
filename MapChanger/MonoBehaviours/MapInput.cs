using System.Diagnostics;
using System.Linq;
using InControl;
using UnityEngine;

namespace MapChanger.MonoBehaviours;

public abstract class MapInput(string name, string mod, PlayerAction defaultInput, float holdMilliseconds = 0)
{
    private readonly Stopwatch _holdTimer = new();

    public string Name { get; } = name;
    public string Mod { get; } = mod;
    public PlayerAction DefaultInput { get; } = defaultInput;
    public float HoldMilliseconds { get; } = holdMilliseconds;

    public KeyCode DebugModOverride { get; private set; } = KeyCode.None;

    public void OnMainUpdate()
    {
        _holdTimer.Reset();

        if (!Dependencies.HasDebugMod)
        {
            return;
        }

        if (DebugModInterop.TryGetBinding(Name, out var binding))
        {
            DebugModOverride = binding;
        }
        else
        {
            DebugModOverride = KeyCode.None;
        }
    }

    internal void Listen()
    {
        if (DebugModOverride is not KeyCode.None)
        {
            if (Input.GetKeyDown(DebugModOverride))
            {
                _holdTimer.Restart();
            }

            if (Input.GetKeyUp(DebugModOverride))
            {
                _holdTimer.Reset();
            }

            if (_holdTimer.IsRunning && _holdTimer.ElapsedMilliseconds >= HoldMilliseconds)
            {
                _holdTimer.Reset();
                DoAction();
            }

            return;
        }

        if (DefaultInput.WasPressed && !CtrlPressed())
        {
            _holdTimer.Restart();
        }

        if (DefaultInput.WasReleased)
        {
            _holdTimer.Reset();
        }

        if (_holdTimer.IsRunning && _holdTimer.ElapsedMilliseconds >= HoldMilliseconds)
        {
            _holdTimer.Reset();
            DoAction();
        }
    }

    public abstract void DoAction();

    public string GetBindingsText()
    {
        if (DebugModOverride is not KeyCode.None)
        {
            return $"[{DebugModOverride}]";
        }

        if (!DefaultInput.Bindings.Any())
        {
            return "UNBOUND";
        }

        var text = $"[{DefaultInput.Bindings.First().Name}]";

        if (
            DefaultInput.Bindings.ElementAtOrDefault(1) is BindingSource secondBinding
            && secondBinding.BindingSourceType == BindingSourceType.DeviceBindingSource
        )
        {
            text += $" / ({secondBinding.Name})";
        }

        return text;
    }

    private static bool CtrlPressed()
    {
        return Input.GetKey("left ctrl") || Input.GetKey("right ctrl");
    }
}
