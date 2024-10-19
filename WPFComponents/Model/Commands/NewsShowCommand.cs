using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class NewsShowCommand : ICommandAction
    {
        public bool CanExecute()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Execute()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "https://news.google.com/?hl=ru",
                UseShellExecute = true // Это позволит системе открыть URL в браузере по умолчанию
            };

            Process.Start(psi);

        }
    }
}
