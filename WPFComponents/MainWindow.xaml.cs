using NAudio.CoreAudioApi;
using System.Collections.ObjectModel;
using System.Windows;
using WPFComponents.Model;
using WPFComponents.Model.Commands;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SettingItem> Settings { get; set; }

        private VoiceCommandProcessor voiceCommandProcessor;

        private AudioWebSocketClient audioWebSocketClient;

        public MainWindow()
        {
            InitializeComponent();

            voiceCommandProcessor = new VoiceCommandProcessor();

            // Создание и регистрация команд
            var openAppCommand = new Command
            {
                Id = 1,
                Name = "OpenApp",
                Phrase = "тест",
                Action = new OpenAppCommand("MyApp")
            };

            voiceCommandProcessor.RegisterCommand(openAppCommand);

            var pressKeyCommand = new Command
            {
                Id = 2,
                Name = "PressKey",
                Phrase = "нажми на клавишу A",
                Action = new PressKeyCommand("A")
            };

            voiceCommandProcessor.RegisterCommand(pressKeyCommand);

            var newsShowCommand = new Command
            {
                Id = 2,
                Name = "ShowNews",
                Phrase = "Покажи новости",
                Action = new NewsShowCommand()
            };

            voiceCommandProcessor.RegisterCommand(newsShowCommand);

            var typeWordCommand = new Command
            {
                Id = 2,
                Name = "TypeWord",
                Phrase = "напечатай слово",
                Action = new PrintWordCommand("Пример")
            };

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

            audioWebSocketClient.SilenceDetected += OnSilenceDetected;



            this.DataContext = this;
        }
        private void OnSilenceDetected(object sender, EventArgs e)
        {
            //voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await audioWebSocketClient.DisconnectAsync();
            voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);
        }
    }
}