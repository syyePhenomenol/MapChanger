using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// A wrapper for MagicUI's Button class.
    /// </summary>
    public abstract class ButtonWrapper
    {
        public readonly string Name;
        public Button Button { get; protected set; }

        public ButtonWrapper(string name)
        {
            Name = name;
        }

        public abstract void Make();

        private protected virtual void OnClickInternal(Button button)
        {
            OnClick();
            PauseMenu.Update();
        }

        protected abstract void OnClick();

        public abstract void Update();
    }
}
