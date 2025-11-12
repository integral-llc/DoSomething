using System;
using System.Runtime.InteropServices;

namespace DoSomething
{
    /// <summary>
    /// Centralized Win32 API P/Invoke declarations
    /// Following Microsoft's recommended naming convention for interop classes
    /// </summary>
    internal static class NativeMethods
    {
        #region Hook Delegates

        internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Hook Constants

        internal const int WH_KEYBOARD_LL = 13;
        internal const int WH_MOUSE_LL = 14;
        internal const int WM_MOUSEMOVE = 0x0200;

        #endregion

        #region Mouse Event Constants

        internal const int MOUSEEVENTF_LEFTDOWN = 0x02;
        internal const int MOUSEEVENTF_LEFTUP = 0x04;
        internal const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        internal const int MOUSEEVENTF_RIGHTUP = 0x10;

        #endregion

        #region Hook Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region Mouse Functions

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        #endregion
    }
}
