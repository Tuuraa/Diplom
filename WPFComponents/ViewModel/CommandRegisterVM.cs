using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WPFComponents.Model;

namespace WPFComponents.ViewModel
{
    class CommandRegisterVM : INotifyPropertyChanged
    {
        private readonly string _settingsPath = "C:\\DiplomUI\\WPFComponents\\settings.json";
        private Dictionary<string, bool?> _settings;

        private ObservableCollection<SettingControlItem> settingControlItems;
        public ObservableCollection<SettingControlItem> SettingControlItems
        {
            get => settingControlItems;
            set
            {
                settingControlItems = value;
                OnPropertyChanged(nameof(SettingControlItems));
            }
        }

        public void ExecuteClosingCommand()
        {
            string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(_settingsPath, json);
        }

        public CommandRegisterVM()
        {
            string json = File.ReadAllText(_settingsPath);

            _settings = JsonConvert.DeserializeObject<Dictionary<string, bool?>>(json);

            //_settingControl = settingControl;
            SettingControlItems = new ObservableCollection<SettingControlItem>
            (
                new[]
                {
                    new SettingControlItem("Открывать сценарии на новом рабочем столе", "Описание 1", _settings["OpenInVD"])
                    {
                        action = () =>
                        {
                            MessageBox.Show("Открывать сценарии на новом рабочем столе");
                        },
                        SettingTitle="OpenInVD"
                    },
                    new SettingControlItem("FanzyZones", "Описание 3", true),
                    new SettingControlItem("File Lock Smith", "Описание 4", true),
                    new SettingControlItem("Host File Editor", "Описание 5", true)
                }

            );

            foreach (var item in SettingControlItems)
            {
                item.PropertyChanged += OnSettingItemPropertyChanged;
            }

        }

        private void OnSettingItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingControlItem.isEnabled))
            {
                var item = sender as SettingControlItem;

                // Обновляем значение в словаре
                if (_settings.ContainsKey(item.SettingTitle))
                {
                    _settings[item.SettingTitle] = item.isEnabled;
                }
            }

            ExecuteClosingCommand();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
