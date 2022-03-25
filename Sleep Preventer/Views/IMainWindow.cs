namespace SleepPreventer.Views
{
    public interface IMainWindow
    {
        void PreventSleep();
        void CloseApp();
        void ToggleWindowVisibility(bool isForceShow);
    }
}