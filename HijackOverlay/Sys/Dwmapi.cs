using System;
using System.Runtime.InteropServices;
using HijackOverlay.Sys.Structs;

namespace HijackOverlay.Sys
{
    public static class Dwmapi
    {
        [DllImport("Dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
    }
}