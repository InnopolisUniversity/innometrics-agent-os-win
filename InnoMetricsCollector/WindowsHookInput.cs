using System;
using System.Runtime.InteropServices;

namespace InnoMetricsCollector
{
    internal class WindowsHookInput
    {
        public delegate IntPtr HookDelegate(
            int Code, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr CallNextHookEx(
            IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);


        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowsHookEx(
            int idHook, HookDelegate lpfn, IntPtr hmod, int dwThreadId);
    }
}