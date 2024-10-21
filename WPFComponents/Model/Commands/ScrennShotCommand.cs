using System.Drawing.Imaging;
using System.Windows;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class ScrennShotCommand : ICommandAction
    {
        public bool CanExecute()
        {
            return true;
        }
        public void Execute()
        {
            var screen = ScreenCapture.CaptureDesktop();
            screen.Save(@"C:\temp\snippetsource.jpg", ImageFormat.Jpeg);
        }
    }
}
