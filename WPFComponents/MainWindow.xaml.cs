using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Model.Interfaces;
using WPFComponents.Model.Utils;
using WPFComponents.Utils;
using WPFComponents.View;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<SettingItem> Settings { get; set; }
        ApplicationContext db = new ApplicationContext();

        private VoiceCommandProcessor voiceCommandProcessor;
        private AudioWebSocketClient audioWebSocketClient;
        private SoundWave soundWave;

        private WaveInEvent waveIn;
        private List<double> samples = new List<double>();
        private const int SampleRate = 44100;
        private const double SensitivityFactor = 1.5;
        private const double HeightMultiplier = 2.0;

        #region Эту хуйню перенести потом в класс комманды для работы с окнами
        private ActiveWindowManager windowManager;

        /// <summary>
        /// Метод тестовый закрывает активное окно на момент запуска !!!Может закрыть VS
        /// </summary>
        private void CheckActiveWindow()
        {
            string activeWindowTitle = windowManager.GetActiveWindowTitle();
            if (!string.IsNullOrEmpty(activeWindowTitle))
            {
                MessageBox.Show("Текущее активное окно: " + activeWindowTitle);

                // Сворачивание активного окна
                //windowManager.MinimizeActiveWindow();

                // Закрытие активного окна
                 windowManager.CloseActiveWindow();
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            soundWave = new SoundWave(MyCanvas, waveLine);

            voiceCommandProcessor = new VoiceCommandProcessor();

            //windowManager = new ActiveWindowManager();

            //CheckActiveWindow();

            db.Database.EnsureCreated();

            var coms = db.Commands.ToList();

            voiceCommandProcessor.RegisterCommand(coms.First());

            voiceCommandProcessor.ProcessVoiceCommand("Открой новости");

            var stop = 5;

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
        }
        private void OnSilenceDetected(object sender, EventArgs e)
        {
            //voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);
        }

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
            /*await audioWebSocketClient.ConnectAsync();
            await audioWebSocketClient.StartRecognitionAsync();*/

            // Запускаем получение результатов распознавания
            //await Task.Run(async () => await audioWebSocketClient.ReceiveRecognitionResultAsync());
            CommandRegister settingWindow = new();
            settingWindow.Show();
        }

        private void CloseApp(object sender, RoutedEventArgs e) => this.Close();

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Ошибка воспроизведения видео: {e.ErrorException.Message}");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await audioWebSocketClient.DisconnectAsync();
            voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

    }
}