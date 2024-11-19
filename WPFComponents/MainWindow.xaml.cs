using NAudio.Wave;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Utils;
using WPFComponents.View;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Props
        public ObservableCollection<SettingItem> Settings { get; set; }
        private ApplicationContext db = new ApplicationContext();

        private VoiceCommandProcessor voiceCommandProcessor;
        WebSocketServer socketServer = new WebSocketServer();
        //private AudioWebSocketClient audioWebSocketClient;
        private SoundWave soundWave;

        private WaveInEvent waveIn;
        private List<double> samples = new List<double>();
        private const int SampleRate = 44100;
        private const double SensitivityFactor = 1.5;
        private const double HeightMultiplier = 2.0;
        #endregion


        public MainWindow()
        {
            StartServer();

            InitializeComponent();
            soundWave = new SoundWave(MyCanvas, waveLine);

            voiceCommandProcessor = new VoiceCommandProcessor();

            db.Database.EnsureCreated();

            #region CommandsAddToDB
            //var wordAc = new PrintWordCommand("привет");

            //var printWord = new Command
            //{
            //    Name = "PrintHello",
            //    Phrases = new List<string> { "Напечатай привет", "Напиши привет" },
            //    Action = new PrintWordCommand("привет"),
            //    Type = wordAc.GetType().Name
            //};

            //var butAc = new PressKeyCommand("X", "");
            //var butpress = new Command
            //{
            //    Name = "Press X",
            //    Phrases = new List<string> { "Нажми X" },
            //    Action = butAc,
            //    Type = butAc.GetType().Name
            //};

            //var mouseAc = new MoveMouseCommand(100, 100, false, "MoveMouseCommand");
            //var mouse = new Command
            //{
            //    Name = "Mouse",
            //    Phrases = new List<string> { "Мышь на 100 и 100" },
            //    Action = mouseAc,
            //    Type = mouseAc.GetType().Name
            //};

            //var hidewindow = new HideWindowCommand();
            //var windowcom = new Command
            //{
            //    Name = "HideWindow",
            //    Phrases = new List<string> { "Сверни окно" },
            //    Action = hidewindow,
            //    Type = hidewindow.GetType().Name
            //};

            //db.Commands.Add(windowcom);
            //db.Commands.Add(butpress);
            //db.Commands.Add(mouse);
            //db.SaveChanges();
            #endregion

            var coms = db.Commands.ToList();

            voiceCommandProcessor.RegisterCommand(coms);

            var stop = 5;

            socketServer.OnTextReceived += (partialText) =>
            {
                Dispatcher.Invoke(() =>
                {
                    voiceCommandProcessor.ProcessVoiceCommand(partialText);
                });
            };

        }

        private async void StartServer() => await socketServer.StartAsync("http://localhost:5001/");

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
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
            /*MessageBox.Show(audioWebSocketClient.GetWebSocketCurrentState().ToString());
            MessageBox.Show(string.Join(" ", ReceiveText));

            await audioWebSocketClient.DisconnectAsync();
            voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);*/
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