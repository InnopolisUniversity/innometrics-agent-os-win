using System;

namespace InnoMetricsCollector
{
    public class MouseTracker : IDisposable
    {
        private const int WH_MOUSE_LL = 14;

        private bool disposed;

        private readonly WindowsHookInput.HookDelegate mouseDelegate;
        private readonly IntPtr mouseHandle;

        public MouseTracker()
        {
            mouseDelegate = MouseHookDelegate;
            mouseHandle = WindowsHookInput.SetWindowsHookEx(WH_MOUSE_LL, mouseDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public event EventHandler<EventArgs> MouseMoved;

        private IntPtr MouseHookDelegate(int Code, IntPtr wParam, IntPtr lParam)
        {
            if (Code < 0)
                return WindowsHookInput.CallNextHookEx(mouseHandle, Code, wParam, lParam);

            if (MouseMoved != null)
                MouseMoved(this, new EventArgs());

            return WindowsHookInput.CallNextHookEx(mouseHandle, Code, wParam, lParam);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (mouseHandle != IntPtr.Zero)
                    WindowsHookInput.UnhookWindowsHookEx(mouseHandle);

                disposed = true;
            }
        }

        ~MouseTracker()
        {
            Dispose(false);
        }
    }
}