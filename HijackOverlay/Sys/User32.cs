using System;
using System.Runtime.InteropServices;

namespace HijackOverlay.Sys
{
    public static class User32
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern long GetWindowLongA(IntPtr hWnd, int nIndex);

        [DllImport("User32.dll")]
        public static extern long SetWindowLongA(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("User32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, ulong dwFlags);

        [DllImport("User32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int xy, uint uflags);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}