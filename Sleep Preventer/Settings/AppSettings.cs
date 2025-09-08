using static SleepPreventer.Tools.KeyboardPInvoke;

namespace SleepPreventer.Settings;

internal class AppSettings
{
    public int Interval { get; set; }
    public KeyCode Key { get; set; }
}
