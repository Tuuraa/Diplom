using H.NotifyIcon;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Uwp.Notifications;
using NAudio.CoreAudioApi;
using NAudio.MediaFoundation;
using NAudio.Wave;
using Nodify.Calculator;
using SkyUtils;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Model.Utils;
using WPFComponents.Services;
using WPFComponents.Utils;
using WPFComponents.View;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Command = SkyUtils.Command;
using Scenario = WPFComponents.DB.Scenario;
using Wpf.Ui.Controls;

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
        private WebSocketServer socketServer;
        private WebsocketMessageController<string> websocketController;
        private SoundWave soundWave;
        private FrozenDictionary<string, Action<string>> _messageHandlers;

        public MainWindow(VoiceCommandProcessor processor, DB.ApplicationContext db)
        {
            StartServer();
            _db = db;
            _voiceCommandProcessor = processor;
            InitializeComponent();

            var imageUri = new Uri("C:\\DiplomUI\\WPFComponents\\Media\\idle.gif");
            var image = new BitmapImage(imageUri);
            ImageBehavior.SetAnimatedSource(this.gifImage, image);

            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                    this,                                    // Window class
                    Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
                    true                                     // Whether to change accents automatically
                );
            };
            _taskbarIcon = this.TrayIcon;
            soundWave = new SoundWave(MyCanvas, waveLine);
            MyCanvas.Visibility = Visibility.Collapsed;


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

            //var moveRight = new MoveMouseCommand(200, 0, true, "MoveMouseCommand"); // Вправо
            //var moveDown = new MoveMouseCommand(0, -200, true, "MoveMouseCommand");  // Вниз
            //var moveLeft = new MoveMouseCommand(-200, 0, true, "MoveMouseCommand");  // Влево
            //var moveUp = new MoveMouseCommand(0, 200, true, "MoveMouseCommand");

            //var squareCommands = new List<Command>
            //{
            //    new Command
            //    {
            //        Name = "MoveRight",
            //        Phrases = new List<string> { "Начни квадрат", "Вправо" },
            //        Action = moveRight,
            //        Type = moveRight.GetType().Name
            //    },
            //    new Command
            //    {
            //        Name = "MoveDown",
            //        Phrases = new List<string> { "Вниз" },
            //        Action = moveDown,
            //        Type = moveDown.GetType().Name
            //    },
            //    new Command
            //    {
            //        Name = "MoveLeft",
            //        Phrases = new List<string> { "Влево" },
            //        Action = moveLeft,
            //        Type = moveLeft.GetType().Name
            //    },
            //    new Command
            //    {
            //        Name = "MoveUp",
            //        Phrases = new List<string> { "Вверх", "Закончить квадрат" },
            //        Action = moveUp,
            //        Type = moveUp.GetType().Name
            //    }
            //};

            //_db.Commands.AddRange(squareCommands);
            //Scenario test = new Scenario();
            //test.Commands = new List<Command>();
            //test.Commands.AddRange(squareCommands);
            //test.Phrases = new List<string> { "Нарисуй квадрат" };
            //test.Name = "Квадрат";

            //var circleCommands = new List<Command>();

            //int radius = 10; // Радиус круга
            //int steps = 36; // Количество шагов (чем больше, тем плавнее круг)
            //double angleStep = 2 * Math.PI / steps;

            //for (int i = 0; i < steps; i++)
            //{
            //    int dx = (int)(radius * Math.Cos(i * angleStep));
            //    int dy = (int)(radius * Math.Sin(i * angleStep));

            //    var moveCommand = new MoveMouseCommand(dx, dy, true, "MoveMouseCommand");

            //    circleCommands.Add(new Command
            //    {
            //        Name = $"MoveStep{i}",
            //        Phrases = new List<string> { $"Шаг {i}" },
            //        Action = moveCommand,
            //        Type = moveCommand.GetType().Name
            //    });
            //}

            //Scenario circle = new Scenario();
            //circle.Commands = new List<Command>();
            //circle.Commands.AddRange(circleCommands);
            //circle.Phrases = new List<string> { "тест" };
            //circle.Name = "Круг";

            //db.Scenarios.Add(circle);


            //_db.Scenarios.Add(test);

            //_db.SaveChanges();

            #endregion


            _db.Database.EnsureCreated();
            //constuctor.Show();

            _voiceCommandProcessor = processor;
            _voiceCommandProcessor.TrayIcon = _taskbarIcon;
            //_voiceCommandProcessor.RegisterCommand(coms);

            websocketController = new WebsocketMessageController<string>(async msg =>
            {
                FadeOut(MyCanvas);
                await _voiceCommandProcessor.ProcessVoiceCommand(msg);
                soundWave.StopMicrophone();
                FadeIn(this.gifImage);
            });

            websocketController.RegisterMessageHandler(new Dictionary<string, Action<string>>
            {
                { "success_wake_word", (_) => StartListen() },
                { "mobile_init", (_) => System.Windows.MessageBox.Show("FLUTTER INIT") },
                { "send_screen", async (_) => { await socketServer.BroadcastAsync(await SendScreen()); } },
                { "mobile_msg", async (msg) => {await _voiceCommandProcessor.ProcessVoiceCommand(msg); } },
            });

            _messageHandlers = websocketController.MessageHandlers;

            socketServer.OnTextReceived += (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    websocketController.ExecuteAction(message, message);
                });
            };

        }

        private void StartListen()
        {
            FadeOut(this.gifImage);
            soundWave.StartMicrophone();
            FadeIn(MyCanvas);
        }

        private void FadeIn(UIElement element, double durationSeconds = 0.5)
        {
            element.Visibility = Visibility.Visible;

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillBehavior = FillBehavior.HoldEnd
            };
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void FadeOut(UIElement element, double durationSeconds = 0.5)
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                FillBehavior = FillBehavior.HoldEnd
            };

            fadeOut.Completed += (s, e) =>
            {
                element.Visibility = Visibility.Hidden;
            };

            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }


        private async void EditorView_CommandsUpdated(List<OperationViewModel> updatedCommands,string name, string phrase)
        {
            try
            {
                List<ICommandAction> actions = new List<ICommandAction>();
                List<Command> commands = new List<Command>();
                foreach (var command in updatedCommands)
                {
                    var action = CommandFactory.CreateCommand((SkyUtils.CommandType)command.CommandType, command);
                    Command new_command = new Command();
                    new_command.Id = 0;
                    new_command.Name = "Temp";
                    new_command.Action = action;
                    new_command.Type = action.GetType().Name;
                    new_command.Phrases = new List<string>();
                    commands.Add(new_command);
                }
                Scenario scenario = new Scenario();
                scenario.Name = name;
                scenario.Phrases = new List<string> { phrase };
                scenario.Commands = commands;
                _db.Scenarios.Add(scenario);
                _db.SaveChanges();
                await _voiceCommandProcessor.UpdateMaps();
                FancyBalloon balloon = new FancyBalloon($"Сценарий {name} успешно добавлен");
                balloon.AgreeClicked += (sender, e) =>
                {
                    TrayIcon.CloseBalloon();
                };
                        TrayIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, 50000);
            }
            catch
            {
                System.Windows.MessageBox.Show("Ошибка добавления сценария");
            }
        }
        private async Task<byte[]> SendScreen()
        {
            Rectangle bound = Screen.PrimaryScreen.Bounds;

            using (Bitmap bitmap = new Bitmap(bound.Width, bound.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bound.Size);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private async void StartServer()
        {

            string ip = GetLocalIPv4();
            socketServer = new WebSocketServer();
            await socketServer.StartAsync($"http://{ip}:5001/");
        }

        static string GetLocalIPv4()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Убедимся, что адаптер активен и используется для интернета
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    var ipProps = ni.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ip.Address))
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return "IP not found";
        }


        //private async void StartServer() => await socketServer.StartAsync("http://192.168.0.15:5001/");
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            CommandRegister settingWindow = new();
            settingWindow.Show();
        }
        private void OpenDocs(object sender, RoutedEventArgs e)
        {
            var url = "https://github.com/Tuuraa/Diplom";
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                
            }

        }
        private void MinimizeToTray(object sender, RoutedEventArgs e)
        {
            _originalLeft = this.Left;
            _originalTop = this.Top;

            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            var trayLeft = screenWidth - 50;
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
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Nodify.Calculator.EditorView constuctor = new Nodify.Calculator.EditorView();
            constuctor.Title = "Констуктор сценариев";
            constuctor.CommandsUpdated += EditorView_CommandsUpdated;
            constuctor.Show();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            //Theme.Apply(ThemeType.Dark);
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
              Wpf.Ui.Appearance.ApplicationTheme.Light, // Theme type
              Wpf.Ui.Controls.WindowBackdropType.Mica,  // Background type
              true                                      // Whether to change accents automatically
            );

        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            FadeIn(this.gifImage, 2);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
              Wpf.Ui.Appearance.ApplicationTheme.Dark, // Theme type
              Wpf.Ui.Controls.WindowBackdropType.Mica,  // Background type
              true                                      // Whether to change accents automatically
            );
        }
    }
}