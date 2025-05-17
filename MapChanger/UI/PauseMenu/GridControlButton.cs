namespace MapChanger.UI;

public abstract class GridControlButton<T> : MainButton
    where T : ExtraButtonGrid
{
    public bool GridOpen()
    {
        return Layout.GridOpen<T>();
    }

    protected internal override void OnClick()
    {
        Layout.ToggleExtraGrid<T>();
    }
}
