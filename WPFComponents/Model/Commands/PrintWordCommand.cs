using System.Windows;
using WindowsInput;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model.Commands
{
    internal class PrintWordCommand : ICommandAction
    {
        public readonly string Word;
        private readonly InputSimulator _inputSimulator;

        public PrintWordCommand(string word)
        {
            Word = word;
            _inputSimulator = new InputSimulator();
        }

        public bool CanExecute()
        {
            // Добавьте здесь проверку условий, если требуется
            return !string.IsNullOrEmpty(Word);
        }

        public async Task Execute()
        {
            if (!CanExecute())
            {
                // Если команда не может быть выполнена, можно выдать сообщение
                return;
            }

            // Используем InputSimulator для печати слова по буквам
            foreach (char letter in Word)
            {
                _inputSimulator.Keyboard.TextEntry(letter);
            }

            // Добавьте здесь любые действия после печати, если нужно
        }
    }
}
