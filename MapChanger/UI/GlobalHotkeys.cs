using MagicUI.Core;
using UnityEngine;

namespace MapChanger.UI
{
    internal class GlobalHotkeys : MapUILayer
    {
        protected override bool Condition()
        {
            return true;
        }

        public override void BuildLayout()
        {
            Root.ListenForHotkey(KeyCode.M, Settings.ToggleModEnabled, ModifierKeys.Ctrl);

            Root.ListenForHotkey(KeyCode.T, Settings.ToggleMode, ModifierKeys.Ctrl);
        }
    }
}
