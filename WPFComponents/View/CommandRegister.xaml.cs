using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFComponents.ViewModel;

namespace WPFComponents.View
{
    public partial class CommandRegister : Window
    {
        public CommandRegister()
        {
            InitializeComponent();
            DataContext = new CommandRegisterVM();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double scrollAmount = e.Delta > 0 ? -20 : 20;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollAmount);
            e.Handled = true;
        }

    }
}
