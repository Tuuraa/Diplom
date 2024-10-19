using System.IO;
using System.Windows;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class OpenAppCommand : ICommandAction
    {
        private string _pathToexe {  get; set; }

        public OpenAppCommand(string appName)
        {
            _pathToexe = appName;
        }
        public bool CanExecute()
        {
            return true;
            if (File.Exists(_pathToexe)) return true;
            return false;
        }

        public void Execute()
        {
            //Process.Start(_pathToexe);
            MessageBox.Show(_pathToexe);
        }
    }
}
