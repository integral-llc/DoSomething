using System;
using System.Diagnostics;

namespace DoSomething
{
    /// <summary>
    /// Provides system-wide mouse input monitoring using low-level hooks
    /// </summary>
    internal class GlobalMouseHook
    {
        private IntPtr _hookID = IntPtr.Zero;
        private NativeMethods.LowLevelMouseProc _proc;

        public event EventHandler MouseMoved;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr)NativeMethods.WM_MOUSEMOVE)
                {
                    MouseMoved?.Invoke(this, EventArgs.Empty);
                }
            }
            catch
            {
                // Swallow exceptions to prevent hook from breaking
            }

            return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Hook()
        {
            _proc = HookCallback;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookID = NativeMethods.SetWindowsHookEx(
                        NativeMethods.WH_MOUSE_LL,
                        _proc,
                        NativeMethods.GetModuleHandle(curModule.ModuleName),
                        0);
                }
            }
        }

        public void Unhook()
        {
            NativeMethods.UnhookWindowsHookEx(_hookID);
        }
    }
}
