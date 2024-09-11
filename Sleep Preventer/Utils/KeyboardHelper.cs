using System;
using System.Runtime.InteropServices;
using System.Threading;
using static SleepPreventer.Utils.KeyboardPInvoke;

namespace SleepPreventer.Utils
{
    internal static class KeyboardHelper
    {
        const ushort IgnoredShrt = 0;
        const uint IgnoredInt = 0;
        const uint KeyFlagDown = 0;
        const uint KeyFlagUp = 2;

        public static void KeyPress(KeyCode keyCode)
        {
            var inputDown = CreateKeyboardInput(keyCode, KeyFlagDown);
            var inputUp = CreateKeyboardInput(keyCode, KeyFlagUp);

            INPUT[] keystroke = [inputDown, inputUp];
            SendInput((uint)keystroke.Length, keystroke, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void KeyDoublePress(KeyCode keyCode)
        {
            KeyPress(keyCode);
            Thread.Sleep((int)TimeSpan.FromMilliseconds(500).TotalMilliseconds);
            KeyPress(keyCode);
        }

        private static INPUT CreateKeyboardInput(KeyCode keyCode, uint keyFlag)
        {
            INPUT input = new()
            {
                type = SendInputEventType.InputKeyboard,
                mkhi = new MOUSEANDKEYBOARDINPUT
                {
                    ki = new KEYBOARDINPUT
                    {
                        wVk = (ushort)keyCode,
                        wScan = IgnoredShrt,
                        dwFlags = keyFlag,
                        time = IgnoredInt,
                        dwExtraInfo = IntPtr.Zero,
                    }
                }
            };

            return input;
        }
    }
}
