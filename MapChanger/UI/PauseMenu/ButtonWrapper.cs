using MagicUI.Elements;

namespace MapChanger.UI
{
    /// <summary>
    /// A wrapper for MagicUI's Button class.
    /// </summary>
    public abstract class ButtonWrapper(string name)
    {
        public readonly string Name = name;
        public Button Button { get; protected set; }

        public abstract void Make();

        private protected virtual void OnClickInternal(Button button)
        {
            OnClick();
            PauseMenu.Update();
        }

        private protected virtual void OnHover(Button button)
        {
            OnHover();
        }

        private protected virtual void OnUnhover(Button button)
        {
            OnUnhover();
        }

        protected virtual void OnClick()
        {

        }

        protected virtual void OnHover()
        {

        }

        protected virtual void OnUnhover()
        {

        }

        public abstract void Update();
    }
}
