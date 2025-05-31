using System.Windows;
using WindowsInput;
using WPFComponents.Model.Interfaces;
using SkyUtils;

namespace WPFComponents.Model.Commands
{
    public class PrintWordCommand : ICommandAction
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
            return !string.IsNullOrEmpty(Word);
        }

        public async Task Execute()
        {
            if (!CanExecute())
            {
                return;
            }

            foreach (char letter in Word)
            {
                _inputSimulator.Keyboard.TextEntry(letter);
            }

        }
    }
}
