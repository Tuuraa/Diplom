using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFComponents.Model;

namespace WPFComponents.ViewModel
{
    class SettingControlVM: INotifyPropertyChanged
    {
        private SettingControlItem _settingControl;

        public string? Header
        {
            get => _settingControl.Header;
            set
            {
                if (_settingControl.Header != value)
                {
                    _settingControl.Header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

        public string? Description
        {
            get => _settingControl.Description;
            set
            {
                if (_settingControl.Description != value)
                {
                    _settingControl.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool? IsEnable
        {
            get => _settingControl.isEnabled;
            set
            {
                if (_settingControl.isEnabled != value)
                {
                    _settingControl.isEnabled = value;
                    OnPropertyChanged(nameof(IsEnable));
                }
            }
        }

        public SettingControlVM(SettingControlItem controlItem)
        {
            _settingControl = controlItem;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
