using SkyUtils;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class MoveMouseCommand : ICommandAction
    {
        public string CommandType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Click { get; set; } // Флаг для выполнения клика

        [JsonIgnore]
        private readonly InputSimulator _inputSimulator;

        [JsonConstructor]
        public MoveMouseCommand(int x, int y, bool click, string commandType)
        {
            CommandType = commandType;
            X = x;
            Y = y;
            Click = click;
            _inputSimulator = new InputSimulator();
        }

        public bool CanExecute()
        {
            return true;
        }

        public async Task Execute()
        {
            try
            {
                var currentPos = System.Windows.Forms.Cursor.Position;
                int startX = currentPos.X;
                int startY = currentPos.Y;

                int steps = 50; // Количество шагов для плавности
                int delay = 10; // Задержка между шагами (мс)

                if (Click)
                {
                    _inputSimulator.Mouse.LeftButtonDown();
                }

                _inputSimulator.Mouse.MoveMouseBy(X, Y);

                if (Click)
                {
                    _inputSimulator.Mouse.LeftButtonUp();
                }

                //MessageBox.Show($"Мышь плавно перемещена на ({X}, {Y}){(Click ? " с зажатой кнопкой" : "")}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при перемещении мыши: {ex.Message}");
            }
        }
    }
}
