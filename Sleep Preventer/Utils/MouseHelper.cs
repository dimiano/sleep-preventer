using System.Drawing;
using System.Runtime.InteropServices;

namespace SleepPreventer.Utils
{
    internal class MouseHelper
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };

        public static Point GetPosition()
        {
            Win32Point winPoint = default;
            _ = GetCursorPos(ref winPoint);
            return new Point(winPoint.X, winPoint.Y);
        }

        public static void SetPosition(double x, double y)
        {
            _ = SetCursorPos((int)x, (int)y);
        }

        public static void SetPosition(int x, int y)
        {
            _ = SetCursorPos(x, y);
        }

        public static void SetPosition(Point point)
        {
            _ = SetCursorPos(point.X, point.Y);
        }

        public static void MoveByX(int offset)
        {
            Win32Point winPoint = default;
            _ = GetCursorPos(ref winPoint);
            _ = SetCursorPos(winPoint.X + offset, winPoint.Y);
        }

        public static void MoveByY(int offset)
        {
            Win32Point winPoint = default;
            _ = GetCursorPos(ref winPoint);
            _ = SetCursorPos(winPoint.X, winPoint.Y + offset);
        }
    }
}
