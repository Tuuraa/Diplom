namespace WPFComponents.Model.Abstract
{
    internal interface IRelayCommand
    {
        public bool CanExecute(object obj);

        public void Execute();
    }
}
