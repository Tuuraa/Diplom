using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFComponents.Model;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SettingItem> Settings { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Settings = new ObservableCollection<SettingItem>
            {
                new SettingItem("Расширенная вкладка", "Открыть окно для расширенной вкладки", false),
                new SettingItem("Always on top", "Поверх других окон — это быстрый и простой способ закрепить окна сверху", true),
                new SettingItem("Awake", "Поддерживай свой компьютер в активном состоянии", true),
                new SettingItem("Расширенная вкладка", "Открыть окно для расширенной вкладки", false),
                new SettingItem("Always on top", "Поверх других окон — это быстрый и простой способ закрепить окна сверху", true),
                new SettingItem("Awake", "Поддерживай свой компьютер в активном состоянии", true),
            };

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(Settings);
            settingWindow.Show();
        }
        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Ошибка воспроизведения видео: {e.ErrorException.Message}");
        }

    }
}