using System;

namespace InnoMetricsCollector
{
    public class KeyboardTracker : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;

        private readonly WindowsHookInput.HookDelegate _keyBoardDelegate;
        private bool disposed;
        private readonly IntPtr keyBoardHandle;

        public KeyboardTracker()
        {
            _keyBoardDelegate = KeyboardHookDelegate;
            keyBoardHandle = WindowsHookInput.SetWindowsHookEx(
                WH_KEYBOARD_LL, _keyBoardDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public event EventHandler<EventArgs> KeyBoardKeyPressed;

        private IntPtr KeyboardHookDelegate(
            int Code, IntPtr wParam, IntPtr lParam)
        {
            if (Code < 0)
                return WindowsHookInput.CallNextHookEx(
                    keyBoardHandle, Code, wParam, lParam);

            if (KeyBoardKeyPressed != null)
                KeyBoardKeyPressed(this, new EventArgs());

            return WindowsHookInput.CallNextHookEx(
                keyBoardHandle, Code, wParam, lParam);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (keyBoardHandle != IntPtr.Zero)
                    WindowsHookInput.UnhookWindowsHookEx(
                        keyBoardHandle);

                disposed = true;
            }
        }

        ~KeyboardTracker()
        {
            Dispose(false);
        }
    }
}