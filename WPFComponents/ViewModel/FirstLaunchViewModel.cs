using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using WPFComponents.Model;
using WPFComponents.Utils;

namespace WPFComponents.ViewModel
{
    public partial class FirstLaunchViewModel : ObservableObject
    {
        [ObservableProperty]
        private int currentStep = 0;

        [ObservableProperty]
        private int selectedVoiceType = 0;

        [ObservableProperty]
        private double speechSpeed = 50.0;

        [ObservableProperty]
        private bool canGoNext = true;

        [ObservableProperty]
        private bool canGoPrevious = false;

        public event EventHandler<int> CurrentStepChanged;
        public event EventHandler<bool> WindowCloseRequested;

        public ObservableCollection<SettingControlItem> Settings { get; }

        private readonly SettingsManager _settingsManager;


        public FirstLaunchViewModel()
        {
            UpdateNavigationButtons();
            _settingsManager = SettingsManager.Instance;
            Settings = new ObservableCollection<SettingControlItem>
            {
                new SettingControlItem
                {
                    Header = "Открывать в виртуальном рабочем столе",
                    Description = "Запуск сценариев в отдельном экране",
                    IsEnabled = true
                },
                new SettingControlItem
                {
                    Header = "Показывать уведомления",
                    Description = "Отображать всплывающие уведомления",
                    IsEnabled = false
                },
                new SettingControlItem
                {
                    Header = "Отправка логов",
                    Description = "Анонимная отправка логов",
                    IsEnabled = true
                }
            };

        }

        [RelayCommand]
        private void NextStep()
        {
            if (currentStep < 3)
            {
                CurrentStep++;
                UpdateNavigationButtons();
                CurrentStepChanged?.Invoke(this, currentStep);
            }
        }

        [RelayCommand]
        private void PreviousStep()
        {
            if (currentStep > 0)
            {
                CurrentStep--;
                UpdateNavigationButtons();
                CurrentStepChanged?.Invoke(this, currentStep);
            }
        }

        [RelayCommand]
        private void Finish()
        {
            // Здесь можно сохранить настройки
            SaveSettings();

            // Закрываем окно с положительным результатом
            WindowCloseRequested?.Invoke(this, true);
        }

        partial void OnCurrentStepChanged(int value)
        {
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            CanGoPrevious = currentStep > 0;
            CanGoNext = currentStep < 3;
        }

        private void SaveSettings()
        {
            // Здесь можно реализовать сохранение настроек
            // Например, в конфигурационный файл или базу данных

            // Пример логики сохранения:
            // - SelectedVoiceType: тип голоса (0 - женский, 1 - мужской)
            // - SpeechSpeed: скорость речи (0-100)
            // - Разрешения уже предоставлены на предыдущем шаге

            _settingsManager.SetSetting("FirstStart", false);
            _settingsManager.SaveSettings();
        }

        // Дополнительные команды для специфичных действий
        [RelayCommand]
        private void RequestPermissions()
        {
            // Здесь можно реализовать запрос разрешений
            // Например, доступ к микрофону, динамикам и интернету

            // После успешного получения разрешений переходим к следующему шагу
            NextStep();
        }

        // Свойства для биндинга в UI
        public string[] VoiceTypes => new[] { "Женский голос", "Мужской голос" };

        public string WelcomeTitle => "Добро пожаловать!";
        public string WelcomeMessage => "Это поможет тебе быстро настроить голосового ассистента и подготовить его к работе.";

        public string VoiceSettingsTitle => "Настройка голоса";
        public string VoiceSettingsMessage => "Выберите предпочтительные параметры для голосового ассистента";

        public string PermissionsTitle => "Разрешения";
        public string PermissionsMessage => "Предоставьте необходимые разрешения для корректной работы";

        public string CompletionTitle => "Всё готово!";
        public string CompletionMessage => "Голосовой ассистент успешно настроен и готов к использованию. Теперь вы можете начать работу с ним.";
    }
}