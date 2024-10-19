using System.Collections.ObjectModel;
using System.Windows;
using WPFComponents.Model;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SettingItem> Settings { get; set; }

        private AudioWebSocketClient audioWebSocketClient;

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
            audioWebSocketClient = new AudioWebSocketClient("ws://localhost:5000");

            audioWebSocketClient.OnPartialTextReceived += (partialText) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Добавляем новый частичный текст к уже существующему в TextBox
                    RecognitionTextBox.Text += partialText + " ";
                });
            };


            this.DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await audioWebSocketClient.ConnectAsync();
            await audioWebSocketClient.StartRecognitionAsync();

            // Запускаем получение результатов распознавания
            await Task.Run(async () => await audioWebSocketClient.ReceiveRecognitionResultAsync());
            //SettingWindow settingWindow = new SettingWindow(Settings);
            //settingWindow.Show();
        }
        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Ошибка воспроизведения видео: {e.ErrorException.Message}");
        }

    }
}