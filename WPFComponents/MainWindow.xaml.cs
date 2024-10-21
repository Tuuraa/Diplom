using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Utils;

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
        private SoundWave soundWave;

        private WaveInEvent waveIn;
        private List<double> samples = new List<double>();
        private const int SampleRate = 44100;
        private const double SensitivityFactor = 1.5;
        private const double HeightMultiplier = 2.0;

        public MainWindow()
        {
            InitializeComponent();
            soundWave = new SoundWave(MyCanvas, waveLine);

            voiceCommandProcessor = new VoiceCommandProcessor();

            //voiceCommandProcessor.ProcessVoiceCommand("тест");
            // Создание и регистрация команд
            /*var openAppCommand = new Command
            (
                id: 1,
                name: "OpenApp",
                phrase: "тест",
                action: new OpenAppCommand("MyApp", "OpenAppCommand")
            );

            voiceCommandProcessor.RegisterCommand(openAppCommand);*/

            /*var pressKeyCommand = new Command
            (
                id: 3,
                name: "Press B",
                phrase: "нажми на клавишу B",
                action: new PressKeyCommand("B", "PressKeyCommand")
            );

            voiceCommandProcessor.RegisterCommand(pressKeyCommand);*/

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

            bool isSuccesSerialize = voiceCommandProcessor.CommandSerializer();
            

            this.DataContext = this;
        }
        private void OnSilenceDetected(object sender, EventArgs e)
        {
            //voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);
        }

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
            await audioWebSocketClient.ConnectAsync();
            await audioWebSocketClient.StartRecognitionAsync();

            // Запускаем получение результатов распознавания
            //await Task.Run(async () => await audioWebSocketClient.ReceiveRecognitionResultAsync());
            SettingWindow settingWindow = new SettingWindow(Settings);
            settingWindow.Show();
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