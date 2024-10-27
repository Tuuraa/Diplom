using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class ActiveWindowManager
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int SW_MINIMIZE = 6;
        private const uint WM_CLOSE = 0x0010;
        public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public IntPtr GetActiveWindowHandle()
        {
            return GetForegroundWindow();
        }
        public void MinimizeActiveWindow()
        {
            IntPtr handle = GetActiveWindowHandle();
            if (handle != IntPtr.Zero)
            {
                ShowWindow(handle, SW_MINIMIZE);
            }
        }
        public void CloseActiveWindow()
        {
            IntPtr handle = GetActiveWindowHandle();
            if (handle != IntPtr.Zero)
            {
                PostMessage(handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
