using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public abstract class WorldMapStack : MapUILayer
    {
        protected virtual HorizontalAlignment StackHorizontalAlignment { get; } = HorizontalAlignment.Left;
        protected virtual VerticalAlignment StackVerticalAlignment { get; } = VerticalAlignment.Top;

        protected StackLayout Stack { get; private set; }

        public override void BuildLayout()
        {
            Stack = new(Root, "World Map Stack")
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = StackHorizontalAlignment,
                VerticalAlignment = StackVerticalAlignment,
                Spacing = 10f
            };

            if (StackHorizontalAlignment == HorizontalAlignment.Left)
            {
                if (StackVerticalAlignment == VerticalAlignment.Top)
                {
                    Stack.Padding = new(160f, 170f, 0f, 0f);
                }
                else
                {
                    Stack.Padding = new(160f, 0f, 0f, 150f);
                }
            }
            else
            {
                if (StackVerticalAlignment == VerticalAlignment.Top)
                {
                    Stack.Padding = new(0f, 170f, 160f, 0f);
                }
                else
                {
                    Stack.Padding = new(0f, 160f, 0f, 150f);
                }
            }

            BuildStack();
        }

        protected abstract void BuildStack();

        protected override bool Condition()
        {
            return States.WorldMapOpen && Settings.MapModEnabled();
        }
    }
}
