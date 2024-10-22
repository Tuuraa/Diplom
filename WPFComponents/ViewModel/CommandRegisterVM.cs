using System.Collections.ObjectModel;
using System.ComponentModel;
using WPFComponents.Model;

namespace WPFComponents.ViewModel
{
    class CommandRegisterVM : INotifyPropertyChanged
    {

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

        public CommandRegisterVM()
        {
            //_settingControl = settingControl;
            SettingControlItems = new ObservableCollection<SettingControlItem>
            (
                new[]
                {
                    new SettingControlItem("Цветоподборщик", "Описание 1", true),
                    new SettingControlItem("Переменные среды", "Описание 2", false),
                    new SettingControlItem("FanzyZones", "Описание 3", true),
                    new SettingControlItem("File Lock Smith", "Описание 4", true),
                    new SettingControlItem("Host File Editor", "Описание 5", true),


                }

            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
