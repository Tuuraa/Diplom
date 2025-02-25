using H.NotifyIcon;
using Microsoft.Toolkit.Uwp.Notifications;
using NAudio.CoreAudioApi;
using NAudio.MediaFoundation;
using NAudio.Wave;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WPFComponents.DB;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Services;
using WPFComponents.Utils;
using WPFComponents.View;

namespace WPFComponents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double _originalLeft, _originalTop;
        DB.ApplicationContext _db;
        private TaskbarIcon _taskbarIcon;

        private VoiceCommandProcessor _voiceCommandProcessor;
        WebSocketServer socketServer = new WebSocketServer();
        private SoundWave soundWave;

        public MainWindow(VoiceCommandProcessor processor, DB.ApplicationContext db)
        {
            StartServer();
            _db = db;
            InitializeComponent();
            _taskbarIcon = this.TrayIcon;
            soundWave = new SoundWave(MyCanvas, waveLine);

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

            _db.Database.EnsureCreated();

            var coms = _db.Commands.ToList();

            _voiceCommandProcessor = processor;
            _voiceCommandProcessor.TrayIcon = _taskbarIcon;
            //_voiceCommandProcessor.RegisterCommand(coms);

            socketServer.OnTextReceived += (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (message == "success")
                    {
                        soundWave.StartMicrophone();
                        //SetImgConfig(isActiveMicro, location + "micro_on.png", height: 22);
                    }
                    else
                    {
                        _voiceCommandProcessor.ProcessVoiceCommand(message);
                        soundWave.StopMicrophone();
                        //SetImgConfig(isActiveMicro, location + "micro_off.png");
                    }

                });
            };

        }

        private async void StartServer() => await socketServer.StartAsync("http://localhost:5001/");

        private async void OpenSettings(object sender, RoutedEventArgs e)
        {
            CommandRegister settingWindow = new();
            settingWindow.Show();
        }

        private void MinimizeToTray(object sender, RoutedEventArgs e)
        {
            _originalLeft = this.Left;
            _originalTop = this.Top;

            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            var trayLeft = screenWidth - 50; // Перемещение в угол экрана
            var trayTop = screenHeight - 10;

            var moveX = new DoubleAnimation(this.Left, trayLeft, TimeSpan.FromSeconds(0.5));
            var moveY = new DoubleAnimation(this.Top, trayTop, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuadraticEase() };
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));

            moveY.Completed += (s, e) => this.Visibility = Visibility.Hidden;

            this.BeginAnimation(Window.LeftProperty, moveX);
            this.BeginAnimation(Window.TopProperty, moveY);
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            System.Windows.MessageBox.Show($"Ошибка воспроизведения видео: {e.ErrorException.Message}");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.Opacity = 0;

            var moveX = new DoubleAnimation(SystemParameters.WorkArea.Width - 50, _originalLeft, TimeSpan.FromSeconds(0.5));
            var moveY = new DoubleAnimation(SystemParameters.WorkArea.Height - 10, _originalTop, TimeSpan.FromSeconds(0.5)) { EasingFunction = new QuadraticEase() };
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));

            this.BeginAnimation(Window.LeftProperty, moveX);
            this.BeginAnimation(Window.TopProperty, moveY);
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
        }

        private void SymbolIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PhoneConnectWindow phoneConnectWindow = new PhoneConnectWindow();
            phoneConnectWindow.Show();
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