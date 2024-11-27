using System.Windows;
using System.Windows.Controls;
using WPFComponents.Model;
using WPFComponents.ViewModel;

namespace WPFComponents.View
{
    public partial class SettingControl : UserControl
    {
        public SettingControl()
        {
            InitializeComponent();
            //DataContext = new SettingControlVM(new SettingControlItem(Header, Description, isEnable));
        }
    }
}
