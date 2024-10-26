using System.Text.Json.Serialization;
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
            // Проверяем, что координаты находятся в пределах допустимых значений экрана
            return X >= 0 && Y >= 0;
        }

        public void Execute()
        {
            try
            {
                // Перемещаем мышь на указанные координаты
                _inputSimulator.Mouse.MoveMouseBy(X, Y);

                // Выполняем клик, если указано в параметрах
                if (Click)
                {
                    _inputSimulator.Mouse.LeftButtonClick();
                    MessageBox.Show($"Мышь перемещена на ({X}, {Y}) и выполнен клик.");
                }
                else
                {
                    MessageBox.Show($"Мышь перемещена на ({X}, {Y}).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при перемещении мыши на ({X}, {Y}): {ex.Message}");
            }
        }
    }
}
