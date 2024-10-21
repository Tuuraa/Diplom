using System.IO;
using System.Text.Json.Serialization;
using System.Windows;
using WPFComponents.Model.Abstract;

namespace WPFComponents.Model.Commands
{
    internal class OpenAppCommand : CommandBase
    {
        public string CommandType { get; set; }
        public string PathToExe { get; set; }

        [JsonConstructor]
        public OpenAppCommand(string pathToExe, string commandType)
        {
            CommandType = commandType;
            PathToExe = pathToExe;
        }

        //TODO: убрать return true
        public override bool CanExecute()
        {
            return true;
            return File.Exists(PathToExe);
        }

        public override void Execute()
        {
            // Замените на запуск приложения
            MessageBox.Show($"Запуск приложения: {PathToExe}");
            // Process.Start(PathToExe);
        }
    }
}
