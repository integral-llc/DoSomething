using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DoSomething
{
    internal class GlobalMouseHook
    {
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;

        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc;

        public event EventHandler MouseMoved;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEMOVE)
                {
                    MouseMoved?.Invoke(this, EventArgs.Empty);
                }
            }
            catch
            {
                // Swallow exceptions to prevent hook from breaking
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Hook()
        {
            _proc = HookCallback;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}
