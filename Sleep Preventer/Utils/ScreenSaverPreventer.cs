using System;
using System.Runtime.InteropServices;

namespace SleepPreventer.Utils
{
    public class ScreenSaverPreventer
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public static void PreventScreenSaver(bool keepOn)
        {
            var state = EXECUTION_STATE.ES_CONTINUOUS;
            
            if (keepOn)
            {
                state = EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS;
            }

            _ = SetThreadExecutionState(state);
        }
    }
}
