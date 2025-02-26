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
    internal class OpenSiteCommand : ICommandAction
    {
        private Uri _uri {  get; set; }

        public OpenSiteCommand(string uri) { 
            _uri = new Uri(uri);
        }

        public bool CanExecute()
        {
            return true;
        }

        public async Task Execute()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _uri.AbsoluteUri,
                UseShellExecute = true // Это позволит системе открыть URL в браузере по умолчанию
            };

            Process.Start(psi);
        }
    }
}
