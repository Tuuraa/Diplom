using Newtonsoft.Json;
using SkyUtils;
using System;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class DrawCircleCommand : ICommandAction
    {
        public string CommandType { get; set; }
        public int Radius { get; set; } // Радиус круга

        [JsonIgnore]
        private readonly InputSimulator _inputSimulator;

        [JsonConstructor]
        public DrawCircleCommand(int radius, string commandType)
        {
            CommandType = commandType;
            Radius = radius;
            _inputSimulator = new InputSimulator();
        }

        public bool CanExecute()
        {
            return true; // Всегда можно выполнить команду
        }

        public async Task Execute()
        {
            try
            {
                var currentPos = System.Windows.Forms.Cursor.Position;
                int centerX = currentPos.X;
                int centerY = currentPos.Y;

                int steps = 100; // Количество шагов для рисования круга
                double angleIncrement = 2 * Math.PI / steps; // Угол между шагами

                // Начинаем рисовать круг без сдвигов
                _inputSimulator.Mouse.LeftButtonDown();

                // Рисуем круг
                for (int i = 0; i <= steps; i++)
                {
                    double angle = i * angleIncrement;
                    int x = (int)(centerX + Radius * Math.Cos(angle));
                    int y = (int)(centerY + Radius * Math.Sin(angle));

                    // Перемещаем курсор относительно текущего положения
                    int deltaX = x - currentPos.X;
                    int deltaY = y - currentPos.Y;

                    // Если это первая итерация, не сдвигаем курсор, а сразу начинаем рисовать
                    if (i == 0)
                    {
                        // Начинаем с того места, где курсор есть сейчас, без сдвига
                        currentPos = new System.Drawing.Point(x, y);
                    }
                    else
                    {
                        // Перемещаем курсор относительно текущей позиции
                        _inputSimulator.Mouse.MoveMouseBy(deltaX, deltaY);
                    }

                    // Обновляем текущую позицию
                    currentPos = new System.Drawing.Point(x, y);

                    await Task.Delay(10); // Задержка для плавности
                }

                _inputSimulator.Mouse.LeftButtonUp();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при рисовании круга: {ex.Message}");
            }
        }
    }
}