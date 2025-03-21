using System.Windows;
using WPFComponents.ViewModel;

namespace WPFComponents.View
{
    public partial class PhoneConnectWindow : Window
    {
        private PhoneConnectWindowVM ViewModel => DataContext as PhoneConnectWindowVM;

        public PhoneConnectWindow()
        {
            InitializeComponent();
        }
    }
}
