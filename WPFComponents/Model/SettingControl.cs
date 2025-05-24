using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace WPFComponents.Model
{
    public class SettingControlItem : INotifyPropertyChanged
    {
        public string? Header { get; set; }
        public string? SettingTitle { get; set; }
        public string? Description { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public Action? Action { get; set; }

        public ICommand Command { get; }

        public SettingControlItem(string header, string desc, bool isEnabled, Action? action = null)
        {
            Header = header;
            Description = desc;
            IsEnabled = isEnabled;
            Action = action;
        }
        public SettingControlItem() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
