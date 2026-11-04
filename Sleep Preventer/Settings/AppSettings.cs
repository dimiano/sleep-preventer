using static SleepPreventer.Tools.KeyboardPInvoke;

namespace SleepPreventer.Settings;

internal class AppSettings
{
    public const int SecondsPerDay = 24 * 60 * 60;

    public int Interval { get; set; } = 290; // seconds
    public KeyCode Key { get; set; } = KeyCode.SHIFT;
    public string Url { get; set; } = "";

    public static bool IsIntervalValid(int interval) => interval > 0 && interval < SecondsPerDay;
}
