using Microsoft.Toolkit.Uwp.Notifications;
using NAudio.Wave;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
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
        private readonly ApplicationContext _db;
        private VoiceCommandProcessor _voiceCommandProcessor;
        private readonly WebSocketServer _socketServer = new();
        private readonly SoundWave _soundWave;



        public MainWindow(ApplicationContext context, ILoggerService loggerService)
        {
            InitializeComponent();
            _soundWave = new SoundWave(MyCanvas, waveLine);
            _db = context;

            //ScenarioBuilder scenario = new ScenarioBuilder();
            //scenario.Show();

            InitializeDatabase();
            InitializeVoiceCommandProcessor();
            InitializeWebSocketServer();

            LogStart(loggerService);

            var logs = _db.Logs.ToArray();
            var stop = 5;


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

            //_db.Commands.Add(windowcom);
            //_db.Commands.Add(butpress);
            //_db.Commands.Add(mouse);
            //_db.SaveChanges();
            #endregion
        }

        private async void LogStart(ILoggerService loggerService)
        {
            await loggerService.LogToDatabaseAsync(
                "MainWindowLoaded",
                "MainWindow initialized",
                "Success");

            //MessageBox.Show("Log entry successfully added!");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*MessageBox.Show(audioWebSocketClient.GetWebSocketCurrentState().ToString());
            MessageBox.Show(string.Join(" ", ReceiveText));

            await audioWebSocketClient.DisconnectAsync();
            voiceCommandProcessor.ProcessVoiceCommand(RecognitionTextBox.Text);*/

        }

        #region Инициализация компонентов

        /// <summary>
        /// Инициализация базы данных.
        /// </summary>
        private void InitializeDatabase()
        {
            _db.Database.EnsureCreated();
        }

        /// <summary>
        /// Инициализация процессора голосовых команд.
        /// </summary>
        private void InitializeVoiceCommandProcessor()
        {
            var commands = _db.Commands.ToList();
            _voiceCommandProcessor = new VoiceCommandProcessor(commands);
        }

        /// <summary>
        /// Запуск WebSocket сервера и обработка входящих сообщений.
        /// </summary>
        private async void InitializeWebSocketServer()
        {
            _socketServer.OnTextReceived += ProcessVoiceCommand;
            await _socketServer.StartAsync("http://localhost:5001/");
        }

        #endregion

        #region Обработчики событий

        /// <summary>
        /// Открытие окна настроек.
        /// </summary>
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settingWindow = new CommandRegister();
            settingWindow.Show();
        }

        /// <summary>
        /// Закрытие приложения.
        /// </summary>
        private void CloseApp(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Обработка ошибок воспроизведения медиа.
        /// </summary>
        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show($"Ошибка воспроизведения видео: {e.ErrorException.Message}");
        }

        /// <summary>
        /// Перемещение окна по клику мыши.
        /// </summary>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Обработка входящего текста с WebSocket.
        /// </summary>
        private void ProcessVoiceCommand(string partialText)
        {
            Dispatcher.Invoke(() => _voiceCommandProcessor.ProcessVoiceCommand(partialText));
        }

        #endregion
    }
}
