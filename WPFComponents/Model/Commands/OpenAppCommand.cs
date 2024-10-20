using System.IO;
using System.Text.Json.Serialization;
using System.Windows;

namespace WPFComponents.Model.Commands
{
    internal class OpenAppCommand : CommandBase
    {
        public string CommandType => "OpenAppCommand";
        public string PathToExe { get; private set; }

        [JsonConstructor]
        public OpenAppCommand(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                throw new ArgumentException("Имя приложения не может быть пустым", nameof(appName));
            PathToExe = appName;
        }

        public override bool CanExecute()
        {
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
