using System.Diagnostics;
using UnityEngine;

namespace MapChanger.Input;

public abstract class BindableInput
{
    private readonly Stopwatch _holdTimer = new();
    private readonly AbstractInputSource _defaultInput;
    private readonly DebugModKeyInputSource _debugModInput;

    private AbstractInputSource _currentInput;

    public BindableInput(string name, string category, AbstractInputSource defaultInput, float holdMilliseconds = 0)
    {
        Name = name;
        Category = category;
        _defaultInput = defaultInput;
        HoldMilliseconds = holdMilliseconds;

        if (Dependencies.HasDebugMod)
        {
            _debugModInput = new(name);
        }
    }

    public string Name { get; }
    public string Category { get; }
    public float HoldMilliseconds { get; }

    private protected AbstractInputSource CurrentInput
    {
        get => _currentInput;
        private set
        {
            if (_currentInput != value)
            {
                MapChangerMod.Instance.LogFine($"Updating input source of {Name} to {value.GetType()}");
                _currentInput = value;
                _holdTimer.Reset();
            }
        }
    }

    public abstract bool UseCondition();

    public abstract bool ActiveCondition();

    public abstract bool ModifierKeyCondition();

    public abstract void DoAction();

    public abstract string GetBindingsText();

    internal void Update()
    {
        UpdateCurrentInput();

        if (!ActiveCondition())
        {
            if (_holdTimer.IsRunning)
            {
                _holdTimer.Reset();
            }

            return;
        }

        if (CurrentInput.GetPressed() && ModifierKeyCondition())
        {
            if (HoldMilliseconds is 0f)
            {
                DoAction();
                return;
            }

            _holdTimer.Restart();
        }

        if (CurrentInput.GetReleased())
        {
            _holdTimer.Reset();
        }

        if (_holdTimer.IsRunning && _holdTimer.ElapsedMilliseconds >= HoldMilliseconds)
        {
            _holdTimer.Reset();
            DoAction();
        }
    }

    private void UpdateCurrentInput()
    {
        if (_debugModInput is DebugModKeyInputSource debug)
        {
            if (debug.UpdateKeyBind())
            {
                MapChangerMod.Instance.LogFine($"Updated DebugMod binding of {Name} to {debug.GetBindingsText()}");
            }

            if (debug.Key is not KeyCode.None)
            {
                CurrentInput = debug;
                return;
            }
        }

        CurrentInput = _defaultInput;
    }
}
