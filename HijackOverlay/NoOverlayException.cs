using System;

namespace HijackOverlay
{
    public class NoOverlayException : Exception
    {
        public NoOverlayException() : base("Overlay Window not found")
        {
        }
    }
}