using System;
using System.Runtime.InteropServices;

namespace SleepPreventer.Utils;

internal static partial class ScreenSaverPreventer
{
    [Flags]
    private enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    public static void PreventScreenSaver(bool keepActive)
    {
        if (keepActive)
        {
            var keepDisplayOn = EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS;
            SetThreadExecutionState(keepDisplayOn);

            //var preventOsSleepAndHibernation = EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS;
            //SetThreadExecutionState(preventSleepAndHibernation);
        }
    }
}
