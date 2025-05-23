using System.Collections.ObjectModel;
using System.ComponentModel;
using WPFComponents.Model;

namespace WPFComponents.ViewModel
{
    public class CommandRegisterVM : INotifyPropertyChanged
    {
        public ObservableCollection<SettingTab> Tabs { get; set; }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged(nameof(SelectedTabIndex));
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }

        public SettingTab SelectedTab => Tabs[SelectedTabIndex];

        public CommandRegisterVM()
        {
            Tabs = new ObservableCollection<SettingTab>
            {
                new("Общее", new()
                {
                    new("Открывать в виртуальном рабочем столе", "Запуск сценариев в отдельном экране", true),
                    new("Показывать уведомления", "Отображать всплывающие уведомления", false),
                    new("Отправка логов", "Анонимная отправка логов", false)
                }),
                new("Нейросеть", new()
                {
                    new("Локальная модель", "Использовать локальную версию ИИ", false),
                    new("GPU ускорение", "Ускорение с помощью видеокарты", true)
                }),
                new("Команды", new()
                {
                    new("Автосохранение", "Сохранять команды при изменениях", true)
                }),
                new("Прочее", new()
                {
                    new("Режим разработчика", "Куча крутых фишек", false),
                    new("Версия", "Номер сборки альфа 0.001", false),
                    new("Разработчики", "ZXC Team", false),
                })
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class SettingTab
    {
        public string Name { get; set; }
        public ObservableCollection<SettingControlItem> Settings { get; set; }

        public SettingTab(string name, ObservableCollection<SettingControlItem> settings)
        {
            Name = name;
            Settings = settings;
        }
    }
}
