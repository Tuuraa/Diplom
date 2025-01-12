using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace WPFComponents.Utils
{
    internal class NotifyIconService
    {
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;


        public NotifyIconService(string icon_location, MainWindow window)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(icon_location + "Icons\\icon.ico"),
                Visible = true,
                Text = "Sky AI"
            };

            _mainWindow = window;
            _notifyIcon.ContextMenuStrip = SetupContextMenu();
        }

        private ContextMenuStrip SetupContextMenu()
        {
            var _contextMenu = new ContextMenuStrip();
            
            _contextMenu.Items.Add(AddStripMenuItem("Выход", (object? sender, EventArgs e) =>
                {
                    _notifyIcon.Dispose();
                    Application.Current.Shutdown();
                })
             );

            _contextMenu.Items.Add(AddStripMenuItem("Открыть", (object? sender, EventArgs e) =>
                {
                    if (_mainWindow.WindowState == WindowState.Minimized || !_mainWindow.IsVisible)
                    {
                        _mainWindow.Show();
                        _mainWindow.WindowState = WindowState.Normal;
                    }

                    _mainWindow.Show();
                    _mainWindow.WindowState = WindowState.Normal;
                    _mainWindow.Activate();
                    _mainWindow.Focus();
                })
            );

            return _contextMenu;
        }

        private ToolStripMenuItem AddStripMenuItem(string name, EventHandler action)
        {
            var menuItem = new ToolStripMenuItem(name);
            menuItem.Click += action;

            return menuItem;
        }

        public void CloseApp() => _notifyIcon.Dispose();

    }
}
