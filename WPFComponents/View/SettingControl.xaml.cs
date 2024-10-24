using System.Windows.Controls;
using WPFComponents.Model;
using WPFComponents.ViewModel;

namespace WPFComponents.View
{
    public partial class SettingControl : UserControl
    {
        public string Header { get; set; }
        public string Description { get; set; }
        public bool isEnabled { get; set; }
        public SettingControl()
        {
            InitializeComponent();
            //DataContext = new SettingControlVM(new SettingControlItem(Header, Description, isEnable));
        }
    }
}
