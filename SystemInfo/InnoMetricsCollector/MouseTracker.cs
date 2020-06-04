using System;

namespace InnoMetricsCollector
{
    public class MouseTracker : IDisposable
    {
        public event EventHandler<EventArgs> MouseMoved;

        private WindowsHookInput.HookDelegate mouseDelegate;
        private IntPtr mouseHandle;
        private const Int32 WH_MOUSE_LL = 14;

        private bool disposed;

        public MouseTracker()
        {
            mouseDelegate = MouseHookDelegate;
            mouseHandle = WindowsHookInput.SetWindowsHookEx(WH_MOUSE_LL, mouseDelegate, IntPtr.Zero, 0);
        }

        private IntPtr MouseHookDelegate(Int32 Code, IntPtr wParam, IntPtr lParam)
        {
            if (Code < 0)
                return WindowsHookInput.CallNextHookEx(mouseHandle, Code, wParam, lParam);

            if (MouseMoved != null)
                MouseMoved(this, new EventArgs());

            return WindowsHookInput.CallNextHookEx(mouseHandle, Code, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
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
