namespace MapChanger
{
    /// <summary>
    /// Abstract class for doing stuff when entering a save when quitting back to menu.
    /// </summary>
    public abstract class HookModule
    {
        public abstract void OnEnterGame();
        public abstract void OnQuitToMenu();
    }
}
