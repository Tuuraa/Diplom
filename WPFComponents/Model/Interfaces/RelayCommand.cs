using Wpf.Ui.Input;

namespace WPFComponents.Model.Interfaces
{
    internal class RelayCommand: IRelayCommand
    {

        public readonly Action<object> _execute;
        public readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute != null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);


        //Хуйня для обновления состоянеия команды 
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
