using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyUtils;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    public class OpenSiteCommand : ICommandAction
    {
        public string Url {  get; set; }

        public OpenSiteCommand(string uri) { 
            Url = uri;
        }

        public bool CanExecute()
        {
            return true;
        }

        public async Task Execute()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = new Uri(Url).AbsoluteUri,
                UseShellExecute = true // Это позволит системе открыть URL в браузере по умолчанию
            };

            Process.Start(psi);
        }
    }
}
