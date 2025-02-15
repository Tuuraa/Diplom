using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFComponents.Model.Interfaces;
using WPFComponents.Model.Utils;

namespace WPFComponents.Model.Commands
{
    internal class HideWindowCommand : ICommandAction
    {
        private ActiveWindowManager ActiveWindowManager = new ActiveWindowManager();
        public bool CanExecute()
        {
            string activeWindowTitle = ActiveWindowManager.GetActiveWindowTitle();
            if (!string.IsNullOrEmpty(activeWindowTitle))
            {
                return true;
            }
            return false;
        }
        public async Task Execute()
        {
            ActiveWindowManager.MinimizeActiveWindow();
        }

       
    }
}
