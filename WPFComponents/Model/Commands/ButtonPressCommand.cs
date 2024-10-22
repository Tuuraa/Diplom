using System.Text.Json.Serialization;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using WPFComponents.Model.Abstract;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class PressKeyCommand : CommandBase
    {
        public string CommandType { get; set; }
        public string Key { get; set; }

        [JsonIgnore]
        private readonly InputSimulator _inputSimulator;

        [JsonConstructor]
        public PressKeyCommand(string key, string commandType)
        {
            CommandType = commandType;
            Key = key;
            _inputSimulator = new InputSimulator();
        }

        public override bool CanExecute()
        {
            return GetVirtualKeyCode(Key) != null;
        }

        public override void Execute()
        {
            try
            {
                VirtualKeyCode? keyCode = GetVirtualKeyCode(Key);

                if (keyCode.HasValue)
                {
                    _inputSimulator.Keyboard.KeyPress(keyCode.Value);
                    MessageBox.Show($"Нажата клавиша: {Key}");
                }
                else
                {
                    MessageBox.Show($"Неизвестная клавиша: {Key}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при нажатии клавиши {Key}: {ex.Message}");
            }
        }

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
