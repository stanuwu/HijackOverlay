using System.Runtime.InteropServices;

namespace HijackOverlay.Sys
{
    public class Kernel32
    {
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(Overlay.EventHandler handler, bool add);
    }
}