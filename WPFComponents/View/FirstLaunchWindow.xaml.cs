using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using WPFComponents.ViewModel;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace WPFComponents.View
{
    public partial class FirstLaunchWindow : FluentWindow
    {
        private readonly FirstLaunchViewModel _viewModel;
        private readonly ServiceProvider _serviceProvider;

        public FirstLaunchWindow(ServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = new FirstLaunchViewModel();
            DataContext = _viewModel;
            _serviceProvider = serviceProvider;

            // Подписываемся на события ViewModel
            _viewModel.CurrentStepChanged += OnCurrentStepChanged;
            _viewModel.WindowCloseRequested += OnWindowCloseRequested;
        }

        private void OnCurrentStepChanged(object sender, int currentStep)
        {
            MainTabControl.SelectedIndex = currentStep;
            UpdateStepIndicators(currentStep);
        }

        private void OnWindowCloseRequested(object sender, bool dialogResult)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Close();
        }

        private void UpdateStepIndicators(int currentStep)
        {
            var stepIndicators = new Border[] { Step1Indicator, Step2Indicator, Step3Indicator, Step4Indicator };

            for (int i = 0; i < stepIndicators.Length; i++)
            {
                if (i < currentStep)
                {
                    // Завершенный шаг - зеленый
                    stepIndicators[i].Background = TryFindResource("SystemFillColorSuccessBrush") as System.Windows.Media.Brush
                                                   ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 137, 62));
                    if (stepIndicators[i].Child is TextBlock tb)
                    {
                        tb.Foreground = System.Windows.Media.Brushes.White;
                    }
                    else if (stepIndicators[i].Child is SymbolIcon si)
                    {
                        si.Foreground = System.Windows.Media.Brushes.White;
                    }
                }
                else if (i == currentStep)
                {
                    // Текущий шаг - акцентный цвет
                    stepIndicators[i].Background = TryFindResource("AccentFillColorDefaultBrush") as System.Windows.Media.Brush
                                                   ?? TryFindResource("SystemAccentColorBrush") as System.Windows.Media.Brush
                                                   ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 120, 212));
                    if (stepIndicators[i].Child is TextBlock tb)
                    {
                        tb.Foreground = System.Windows.Media.Brushes.White;
                    }
                    else if (stepIndicators[i].Child is SymbolIcon si)
                    {
                        si.Foreground = System.Windows.Media.Brushes.White;
                    }
                }
                else
                {
                    // Неактивный шаг - серый
                    stepIndicators[i].Background = TryFindResource("ControlFillColorDisabledBrush") as System.Windows.Media.Brush
                                                   ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(249, 249, 249));
                    if (stepIndicators[i].Child is TextBlock tb)
                    {
                        tb.Foreground = TryFindResource("TextFillColorDisabledBrush") as System.Windows.Media.Brush
                                        ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 161, 161));
                    }
                    else if (stepIndicators[i].Child is SymbolIcon si)
                    {
                        si.Foreground = TryFindResource("TextFillColorDisabledBrush") as System.Windows.Media.Brush
                                        ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 161, 161));
                    }
                }
            }
        }
    }
}