using MagicUI.Core;

namespace MapChanger.UI
{
    public abstract class MapUILayer
    {
        public LayoutRoot Root { get; private set; }

        internal void Build()
        {
            if (Root == null)
            {
                Root = new(true, $"{GetType().Name} Root")
                {
                    VisibilityCondition = Condition
                };

                BuildLayout();
            }
        }

        internal void Destroy()
        {
            Root?.Destroy();
            Root = null;
        }

        protected abstract bool Condition();

        public abstract void BuildLayout();

        public virtual void Update() { }
    }
}
