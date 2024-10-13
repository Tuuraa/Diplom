using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFComponents.Model;

namespace WPFComponents
{
    /// <summary>
    /// Логика взаимодействия для SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public ObservableCollection<SettingItem> Settings { get; set; }
        public SettingWindow(ObservableCollection<SettingItem> settings)
        {
            InitializeComponent();
            Settings = settings;
            this.DataContext = this;
        }
    }
}
