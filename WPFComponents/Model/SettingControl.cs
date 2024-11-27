using System.ComponentModel;
using System.Windows.Input;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model
{
    public class SettingControlItem: INotifyPropertyChanged
    {
        public string? Header { get; set; }
        public string? SettingTitle { get; set; }
        public string? Description { get; set; }

        private bool? _isEnabled;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool? isEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(isEnabled));
                }
            }
        }

        public Action action { get; set; }
        public ICommand Command { get; }

        public SettingControlItem(string header, string desc, bool? is_enable)
        {
            Header = header; Description = desc; isEnabled = is_enable;

            Command = new RelayCommand(_ => action?.Invoke(), _ => true) ?? null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
