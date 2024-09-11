namespace SleepPreventer.Views;

public interface IMainWindow
{
    void CloseApp();
    void PreventSleep();
    void ToggleWindowVisibility(bool isForceShow);
    void UpdateSettings();
}