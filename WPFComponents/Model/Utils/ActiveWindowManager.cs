using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model.Utils
{
    public class ActiveWindowManager
    {
        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

        private const int SW_MINIMIZE = 6;
        private const uint WM_CLOSE = 0x0010;
        public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            nint handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public nint GetActiveWindowHandle()
        {
            return GetForegroundWindow();
        }
        public void MinimizeActiveWindow()
        {
            nint handle = GetActiveWindowHandle();
            if (handle != nint.Zero)
            {
                ShowWindow(handle, SW_MINIMIZE);
            }
        }
        public void CloseActiveWindow()
        {
            nint handle = GetActiveWindowHandle();
            if (handle != nint.Zero)
            {
                PostMessage(handle, WM_CLOSE, nint.Zero, nint.Zero);
            }
        }

    }
}
