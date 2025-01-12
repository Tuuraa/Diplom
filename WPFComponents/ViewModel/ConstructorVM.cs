using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Input;
using WPFComponents.Model;

namespace WPFComponents.ViewModel
{
    public class ConstructorVM
    {
        public ObservableCollection<NodeViewModel> Nodes { get; } = new();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new();
        public ObservableCollection<NodeViewModel> AvailableElements { get; } = new ObservableCollection<NodeViewModel>();

        private ConnectorViewModel _pendingConnector;

        public ConstructorVM()
        {
            // Инициализация доступных элементов
            AvailableElements.Add(new NodeViewModel { Title = "Элемент 1" });
            AvailableElements.Add(new NodeViewModel { Title = "Элемент 2" });
            AvailableElements.Add(new NodeViewModel { Title = "Элемент 3" });



            // Создание соединения между узлами
            //Connections.Add(new ConnectionViewModel(node1.Output[0], node2.Input[0]));


        }
        public void StartConnection(ConnectorViewModel connector)
        {
            _pendingConnector = connector;
        }

        public void CompleteConnection(ConnectorViewModel connector)
        {
            if (_pendingConnector != null && _pendingConnector != connector)
            {
                Connections.Add(new ConnectionViewModel
                (
                    _pendingConnector,
                     connector
                ));
            }
            //Connections.Add(new ConnectionViewModel(Nodes[0].Output[0], Nodes[1].Input[0]));
            _pendingConnector = null;
        }
    }
    public class ConnectorViewModel : ObservableObject
    {
        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                OnPropertyChanged(nameof(Anchor));
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        public string Title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class NodeViewModel : ObservableObject
    {
        public string Title { get; set; }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public ObservableCollection<ConnectorViewModel> Input { get; set; } = new();
        public ObservableCollection<ConnectorViewModel> Output { get; set; } = new();
        public NodeViewModel()
        {
            // Добавляем точки
            Input.Add(new ConnectorViewModel { Title = "Вход 1" });
            Output.Add(new ConnectorViewModel { Title = "Выход 1" });
        }
    }

    public class ConnectionViewModel : ObservableObject
    {
        public ConnectionViewModel(ConnectorViewModel source, ConnectorViewModel target)
        {
            Source = source;
            Target = target;

            // Устанавливаем флаг IsConnected для соединённых коннекторов
            Source.IsConnected = true;
            Target.IsConnected = true;
        }

        public ConnectorViewModel Source { get; set; }
        public ConnectorViewModel Target { get; set; }
    }



}
