using static SleepPreventer.Tools.KeyboardPInvoke;

namespace SleepPreventer.Settings;

internal class AppSettings
{
    public int Interval { get; set; } = 290; // seconds
    public KeyCode Key { get; set; } = KeyCode.SHIFT;
}
