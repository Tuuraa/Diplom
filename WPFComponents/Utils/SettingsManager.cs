using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace WPFComponents.Utils
{
    public class SettingsManager
    {
        private static readonly Lazy<SettingsManager> _instance = new(() => new SettingsManager());
        public static SettingsManager Instance => _instance.Value;

        private readonly string _settingsPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private AppSettings _settings;

        private SettingsManager()
        {
            // Путь к файлу настроек в папке пользователя
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "SkyAI");
            Directory.CreateDirectory(appFolder);
            _settingsPath = Path.Combine(appFolder, "settings.json");

            // Настройки JSON сериализации
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            LoadSettings();
        }

        public AppSettings Settings => _settings ??= new AppSettings();

        // Загрузка настроек из файла
        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);

                    if (_settings == null)
                    {
                        _settings = new AppSettings();
                    }
                }
                else
                {
                    _settings = new AppSettings();
                    SaveSettings(); // Создаем файл с настройками по умолчанию
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                _settings = new AppSettings();
            }
        }

        // Сохранение настроек в файл
        public void SaveSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(_settings, _jsonOptions);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Сброс настроек к значениям по умолчанию
        public void ResetSettings()
        {
            _settings = new AppSettings();
            SaveSettings();
        }

        // Получение значения настройки по ключу
        public T GetSetting<T>(string key, T defaultValue = default)
        {
            try
            {
                var property = typeof(AppSettings).GetProperty(key);
                if (property != null)
                {
                    var value = property.GetValue(_settings);
                    return value != null ? (T)value : defaultValue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка получения настройки {key}: {ex.Message}");
            }
            return defaultValue;
        }

        // Установка значения настройки по ключу
        public void SetSetting<T>(string key, T value)
        {
            try
            {
                var property = typeof(AppSettings).GetProperty(key);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(_settings, value);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка установки настройки {key}: {ex.Message}");
            }
        }
    }

    public class AppSettings : INotifyPropertyChanged
    {
        private string _theme = "Light";
        private string _language = "ru-RU";
        private double _windowWidth = 800;
        private double _windowHeight = 600;
        private bool _autoSave = true;
        private string _lastOpenedFile = "";
        private int _maxRecentFiles = 10;
        private bool _firstStart = true;
        private bool _openInVirtualDesktop = true;
        private bool _showNotifications = false;
        private bool _sendLogs = false;

        [JsonPropertyName("theme")]
        public string Theme
        {
            get => _theme;
            set { _theme = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("language")]
        public string Language
        {
            get => _language;
            set { _language = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("windowWidth")]
        public double WindowWidth
        {
            get => _windowWidth;
            set { _windowWidth = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("windowHeight")]
        public double WindowHeight
        {
            get => _windowHeight;
            set { _windowHeight = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("autoSave")]
        public bool AutoSave
        {
            get => _autoSave;
            set { _autoSave = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("lastOpenedFile")]
        public string LastOpenedFile
        {
            get => _lastOpenedFile;
            set { _lastOpenedFile = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("maxRecentFiles")]
        public int MaxRecentFiles
        {
            get => _maxRecentFiles;
            set { _maxRecentFiles = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("FirstStart")]
        public bool FirstStart 
        {
            get => _firstStart;
            set { _firstStart = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("openInVirtualDesktop")]
        public bool OpenInVirtualDesktop
        {
            get => _openInVirtualDesktop;
            set { _openInVirtualDesktop = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("showNotifications")]
        public bool ShowNotifications
        {
            get => _showNotifications;
            set { _showNotifications = value; OnPropertyChanged(); }
        }

        [JsonPropertyName("sendLogs")]
        public bool SendLogs
        {
            get => _sendLogs;
            set { _sendLogs = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
