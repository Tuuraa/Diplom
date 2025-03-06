using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using WPFComponents.Model.Abstract;
using SkyUtils;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class OpenAppCommand : ICommandAction
    {
        public string CommandType { get; set; }
        public string PathToExe { get; set; }
        public OpenAppCommand(string pathToExe)
        {
            CommandType = this.GetType().Name;
            PathToExe = pathToExe;
        }

        //TODO: убрать return true
        public bool CanExecute()
        {
            //return true;
            return File.Exists(PathToExe);
        }

        public async Task Execute()
        {
            // Замените на запуск приложения
            //MessageBox.Show($"Запуск приложения: {PathToExe}");
            Process.Start(PathToExe);
        }
    }
}
