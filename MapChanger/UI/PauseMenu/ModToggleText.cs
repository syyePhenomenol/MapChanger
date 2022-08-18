using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI
{
    internal class ModToggleText : Title
    {
        public ModToggleText() : base("Press Ctrl-M to enable map mod")
        {

        }

        public override void Update()
        {
            if (!Settings.MapModWasEnabled)
            {
                TitleText.Visibility = Visibility.Visible;
            }
            else
            {
                TitleText.Visibility = Visibility.Hidden;
            }
        }
    }
}
