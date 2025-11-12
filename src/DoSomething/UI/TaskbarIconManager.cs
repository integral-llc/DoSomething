using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DoSomething
{
    /// <summary>
    /// Manages taskbar icon overlays based on application state
    /// Uses Windows ITaskbarList3 COM interface
    /// </summary>
    public class TaskbarIconManager : IDisposable
    {
        private readonly ITaskbarList3 _taskbarList;
        private readonly IntPtr _windowHandle;
        private Icon _workingIcon;
        private Icon _pausedIcon;
        private bool _disposed;

        public TaskbarIconManager(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;

            // Initialize COM interface for Windows 7+ taskbar
            try
            {
                _taskbarList = (ITaskbarList3)new CTaskbarList();
                _taskbarList.HrInit();

                // Create overlay icons
                CreateOverlayIcons();
            }
            catch
            {
                // Windows 7+ required, gracefully fail on older systems
                _taskbarList = null;
            }
        }

        public void UpdateIcon(ApplicationState state)
        {
            if (_taskbarList == null || _windowHandle == IntPtr.Zero)
                return;

            try
            {
                switch (state)
                {
                    case ApplicationState.Stopped:
                        _taskbarList.SetOverlayIcon(_windowHandle, IntPtr.Zero, "Stopped");
                        break;

                    case ApplicationState.Working:
                        if (_workingIcon != null)
                            _taskbarList.SetOverlayIcon(_windowHandle, _workingIcon.Handle, "Working");
                        break;

                    case ApplicationState.Paused:
                        if (_pausedIcon != null)
                            _taskbarList.SetOverlayIcon(_windowHandle, _pausedIcon.Handle, "Paused");
                        break;
                }
            }
            catch
            {
                // Ignore errors - not critical functionality
            }
        }

        private void CreateOverlayIcons()
        {
            // Create 16x16 overlay icons for taskbar
            _workingIcon = CreateColoredIcon(Color.Green, 16, 16);
            _pausedIcon = CreateColoredIcon(Color.Orange, 16, 16);
        }

        private Icon CreateColoredIcon(Color color, int width, int height)
        {
            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw filled circle
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 2, 2, width - 4, height - 4);
                }

                // Draw border
                using (Pen pen = new Pen(Color.FromArgb(200, Color.White), 1))
                {
                    g.DrawEllipse(pen, 2, 2, width - 4, height - 4);
                }

                IntPtr hIcon = bitmap.GetHicon();
                Icon icon = Icon.FromHandle(hIcon);
                return icon;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _workingIcon?.Dispose();
            _pausedIcon?.Dispose();

            if (_taskbarList != null && _windowHandle != IntPtr.Zero)
            {
                try
                {
                    _taskbarList.SetOverlayIcon(_windowHandle, IntPtr.Zero, "");
                }
                catch { }

                Marshal.ReleaseComObject(_taskbarList);
            }

            _disposed = true;
        }

        #region COM Interface Declarations

        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class CTaskbarList { }

        [ComImport]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, int tbpFlags);
            [PreserveSig]
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            [PreserveSig]
            void UnregisterTab(IntPtr hwndTab);
            [PreserveSig]
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            [PreserveSig]
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);
            [PreserveSig]
            void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            [PreserveSig]
            void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButton);
            [PreserveSig]
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            [PreserveSig]
            void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
            [PreserveSig]
            void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
            [PreserveSig]
            void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
        }

        #endregion
    }
}
