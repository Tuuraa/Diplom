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
    public class DrawSquareCommand : ICommandAction
    {
        public string CommandType { get; set; }
        public int SideLength { get; set; } // Длина стороны квадрата

        [JsonIgnore]
        private readonly InputSimulator _inputSimulator;

        [JsonConstructor]
        public DrawSquareCommand(int sideLength, string commandType)
        {
            CommandType = commandType;
            SideLength = sideLength;
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

                _inputSimulator.Mouse.LeftButtonDown();

                // Перемещаем по четырем углам квадрата
                // Первый угол (от текущей позиции)
                _inputSimulator.Mouse.MoveMouseBy(SideLength, 0); // Перемещаемся вправо
                await Task.Delay(delay);

                // Второй угол (верхний правый)
                _inputSimulator.Mouse.MoveMouseBy(0, SideLength); // Перемещаемся вниз
                await Task.Delay(delay);

                // Третий угол (нижний правый)
                _inputSimulator.Mouse.MoveMouseBy(-SideLength, 0); // Перемещаемся влево
                await Task.Delay(delay);

                // Четвертый угол (нижний левый)
                _inputSimulator.Mouse.MoveMouseBy(0, -SideLength); // Перемещаемся вверх
                await Task.Delay(delay);

                _inputSimulator.Mouse.LeftButtonUp();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при рисовании квадрата: {ex.Message}");
            }
        }
    }
}
