using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WindowsDesktop;

namespace WPFComponents.Model.Utils
{
    public class VirtualDesktopManager
    {
        public static void MoveWindowToNewDesktop(Window appWindow, VirtualDesktop desktop)
        {
            // Получаем дескриптор окна
            IntPtr windowHandle = new WindowInteropHelper(appWindow).Handle;

            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Не удалось получить хендл окна.");
            }

            // Создаём новый виртуальный рабочий стол
            var newDesktop = VirtualDesktop.Create();

            // Перемещаем окно на новый рабочий стол
            VirtualDesktopHelper.MoveWindowToDesktop(windowHandle, newDesktop.Id);

            // Переключаемся на новый рабочий стол
            newDesktop.Switch();
        }
    }

    [ComImport]
    [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IVirtualDesktopManager
    {
        bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);
        void GetWindowDesktopId(IntPtr topLevelWindow, out Guid desktopId);
        void MoveWindowToDesktop(IntPtr topLevelWindow, Guid desktopId);
    }

    public static class VirtualDesktopHelper
    {
        private static readonly IVirtualDesktopManager VirtualDesktopManager =
            (IVirtualDesktopManager)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("c179334c-4295-40d3-bea1-c654d965605a")));

        public static void MoveWindowToDesktop(IntPtr windowHandle, Guid desktopId)
        {
            if (windowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Неверный дескриптор окна.");
            }

            // Перемещаем окно на указанный рабочий стол
            VirtualDesktopManager.MoveWindowToDesktop(windowHandle, desktopId);
        }
    }
}
