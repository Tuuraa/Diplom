using System.Drawing.Imaging;
using System.Windows;
using WPFComponents.Model.Interfaces;
using SkyUtils;
using WPFComponents.Model.Utils;

namespace WPFComponents.Model.Commands
{
    public class ScrennShotCommand : ICommandAction
    {
        public bool CanExecute()
        {
            return true;
        }
        public async Task Execute()
        {
            var screen = ScreenCapture.CaptureDesktop();
            screen.Save(@"C:\temp\snippetsource.jpg", ImageFormat.Jpeg);
        }
    }
}
