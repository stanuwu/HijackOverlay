using System;
using System.Threading;
using GameOverlay.Windows;
using HijackOverlay.Render;
using HijackOverlay.Render.Buffer;
using HijackOverlay.Render.Shader;
using HijackOverlay.Sys;
using HijackOverlay.Sys.Structs;
using OpenGL;

namespace HijackOverlay
{
    public class Overlay
    {
        public delegate bool EventHandler(CtrlType sig);

        private const string ClassName = "CEF-OSC-WIDGET";
        private const string WindowName = "NVIDIA GeForce Overlay";
        private static EventHandler _handler;

        public Overlay(int width, int height, string className = ClassName, string windowName = WindowName)
        {
            Width = width;
            Height = height;
            Init(className, windowName);
        }

        public static int X { get; private set; }
        public static int Y { get; private set; }
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        private IntPtr WindowHandle { get; set; }
        private IntPtr DeviceContext { get; set; }
        private IntPtr GlContext { get; set; }
        private bool WillClose { get; set; }
        private OverlayWindow? OverlayWindow { get; set; }

        public void Dispose()
        {
            BufferRenderer.Instance.Delete();
            ShaderManager.Instance.Delete();
            Wgl.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Wgl.DeleteContext(DeviceContext);
            DeviceContext = default;
            GlContext = default;
            WindowHandle = default;
            OverlayWindow?.Dispose();
        }

        private bool Handler(CtrlType sig)
        {
            WillClose = true;
            Thread.Sleep(100);
            Environment.Exit(-1);

            return true;
        }

        public void Clear()
        {
            Wgl.MakeCurrent(DeviceContext, GlContext);
            Renderer.SetBlend();
            Gl.ClearColor(0, 0, 0, 0);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Wgl.SwapLayerBuffers(DeviceContext, Wgl.SWAP_MAIN_PLANE);
        }

        private void Init(string className, string windowName)
        {
            _handler += Handler;
            Kernel32.SetConsoleCtrlHandler(_handler, true);

            // Hijack or Create Overlay
            if (className == "Create" && windowName == "Create")
            {
                var overlay = new OverlayWindow();
                overlay.Width = Width;
                overlay.Height = Height;
                overlay.Create();
                WindowHandle = overlay.Handle;
            }
            else
            {
                WindowHandle = User32.FindWindowA(className, windowName);
            }
            if (WindowHandle == IntPtr.Zero) throw new NoOverlayException();
            var info = User32.GetWindowLongA(WindowHandle, -20);
            User32.SetWindowLongA(WindowHandle, -20, info | 0x20);
            var margins = new Margins
            {
                Left = -1,
                Right = -1,
                Top = -1,
                Bottom = -1
            };
            Dwmapi.DwmExtendFrameIntoClientArea(WindowHandle, ref margins);
            User32.SetLayeredWindowAttributes(WindowHandle, 0x00000000, 0xFF, 0x02);
            User32.SetWindowPos(WindowHandle, new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001);
            User32.ShowWindow(WindowHandle, 5);

            Gl.Initialize();
            DeviceContext = User32.GetDC(WindowHandle);

            var pfd = new Wgl.PIXELFORMATDESCRIPTOR(32);
            pfd.cAlphaBits = 8;
            var pfdId = Wgl.ChoosePixelFormat(DeviceContext, ref pfd);
            Wgl.SetPixelFormat(DeviceContext, pfdId, ref pfd);
            GlContext = Wgl.CreateContext(DeviceContext);
            Wgl.MakeCurrent(DeviceContext, GlContext);
        }

        public void StartDraw(int x, int y, int width, int height)
        {
            Wgl.MakeCurrent(DeviceContext, GlContext);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Gl.Viewport(x, y, width, height);
            Renderer.SetBlend();
            Gl.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            Gl.Enable(EnableCap.LineSmooth);
            Gl.ClearColor(0, 0, 0, 0);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void EndDraw()
        {
            if (WillClose)
            {
                Clear();
                Dispose();
            }
            else
            {
                Wgl.SwapLayerBuffers(DeviceContext, Wgl.SWAP_MAIN_PLANE);
            }
        }
    }
}