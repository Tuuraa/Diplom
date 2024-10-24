using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFComponents
{
    /// <summary>
    /// Логика взаимодействия для SettingElement.xaml
    /// </summary>
    public partial class SettingElement : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SettingElement), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SettingElement), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(SettingElement), new PropertyMetadata(true));

        public static readonly DependencyProperty WindowHeightProperty =
            DependencyProperty.Register("WindowHeight", typeof(int), typeof(SettingElement), new PropertyMetadata(100));

        public static readonly DependencyProperty WindowWidthProperty =
            DependencyProperty.Register("WindowWidth", typeof(int), typeof(SettingElement), new PropertyMetadata(500));


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public int WindowHeight
        {
            get { return (int)GetValue(WindowHeightProperty); }
            set { SetValue(WindowHeightProperty, value); }
        }

        public int WindowWidth
        {
            get { return (int)GetValue(WindowWidthProperty); }
            set { SetValue(WindowWidthProperty, value); }
        }

        public SettingElement()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Background = new SolidColorBrush(Colors.Blue);
        }
    }

}
