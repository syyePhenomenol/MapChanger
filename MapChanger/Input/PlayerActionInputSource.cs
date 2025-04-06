using System;
using System.Linq;
using InControl;

namespace MapChanger.Input;

public class PlayerActionInputSource(Func<PlayerAction> getPlayerAction) : AbstractInputSource
{
    private readonly Func<PlayerAction> _getPlayerAction = getPlayerAction;

    public PlayerAction PlayerAction => _getPlayerAction.Invoke();

    public override bool GetPressed()
    {
        return PlayerAction.WasPressed;
    }

    public override bool GetReleased()
    {
        return PlayerAction.WasReleased;
    }

    public override string GetBindingsText()
    {
        if (PlayerAction is null || !PlayerAction.Bindings.Any())
        {
            return "UNBOUND";
        }

        var text = $"[{PlayerAction.Bindings.First().Name}]";

        if (
            PlayerAction.Bindings.ElementAtOrDefault(1) is BindingSource secondBinding
            && secondBinding.BindingSourceType == BindingSourceType.DeviceBindingSource
        )
        {
            text += $" or ({secondBinding.Name})";
        }

        return text;
    }
}
