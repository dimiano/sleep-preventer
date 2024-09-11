using System.Drawing;
using System.Runtime.InteropServices;

namespace SleepPreventer.Utils;

internal partial class MouseHelper
{
    private static bool _flipDirection;

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetCursorPos(int X, int Y);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(ref Win32Point pt);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Point
    {
        public int X;
        public int Y;
    };

    public static Point GetPosition()
    {
        Win32Point winPoint = default;
        GetCursorPos(ref winPoint);
        return new Point(winPoint.X, winPoint.Y);
    }

    public static void SetPosition(double x, double y) => SetCursorPos((int)x, (int)y);

    public static void SetPosition(int x, int y) => SetCursorPos(x, y);

    public static void SetPosition(Point point) => SetCursorPos(point.X, point.Y);

    public static void MoveByX(int offset)
    {
        Win32Point winPoint = default;
        GetCursorPos(ref winPoint);
        SetCursorPos(winPoint.X + offset, winPoint.Y);
    }

    public static void MoveByY(int offset)
    {
        Win32Point winPoint = default;
        GetCursorPos(ref winPoint);
        SetCursorPos(winPoint.X, winPoint.Y + offset);
    }

    public static void MoveCursorBy(int offset)
    {
        // preserve cursor near the original position
        var shift = _flipDirection ? offset : -offset;
        _flipDirection = !_flipDirection;

        MoveByX(shift);
        MoveByY(shift);
    }
}
