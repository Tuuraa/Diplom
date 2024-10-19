using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class PressKeyCommand : ICommandAction
    {
        private string _key { get; set; }
        private InputSimulator _inputSimulator;

        public PressKeyCommand(string key)
        {
            _key = key;
            _inputSimulator = new InputSimulator(); // Инициализация InputSimulator
        }

        public bool CanExecute()
        {
            // Проверка на наличие корректного ключа
            return GetVirtualKeyCode(_key) != null;
        }

        public void Execute()
        {
            try
            {
                // Получение VirtualKeyCode для нажатия
                VirtualKeyCode? keyCode = GetVirtualKeyCode(_key);

                if (keyCode != null)
                {
                    _inputSimulator.Keyboard.KeyPress((VirtualKeyCode)keyCode);
                    MessageBox.Show($"Нажата клавиша: {_key}");
                }
                else
                {
                    MessageBox.Show($"Неизвестная клавиша: {_key}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при нажатии клавиши {_key}: {ex.Message}");
            }
        }

        // Преобразование строки клавиши в VirtualKeyCode
        private VirtualKeyCode? GetVirtualKeyCode(string key)
        {
            return key.ToUpper() switch
            {
                "A" => VirtualKeyCode.VK_A,
                "B" => VirtualKeyCode.VK_B,
                "C" => VirtualKeyCode.VK_C,
                "ENTER" => VirtualKeyCode.RETURN,
                "SPACE" => VirtualKeyCode.SPACE,
                "ESC" => VirtualKeyCode.ESCAPE,
                _ => null
            };
        }
    }
}
