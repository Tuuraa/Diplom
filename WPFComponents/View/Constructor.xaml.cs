using NAudio.CoreAudioApi;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using WPFComponents.ViewModel;

namespace WPFComponents.View
{
    /// <summary>
    /// Логика взаимодействия для Constructor.xaml
    /// </summary>
    public partial class Constructor : Window
    {
        public Constructor()
        {
            InitializeComponent();
            DataContext = new ConstructorVM();
        }
        private void NodifyEditor_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(NodeViewModel)) is NodeViewModel droppedNode)
            {
                var editor = (NodifyEditor)sender;

                var dropPosition = e.GetPosition((IInputElement)sender);

                // Добавляем узел на рабочую область
                var newNode = new NodeViewModel
                {
                    Title = droppedNode.Title,
                    Location = new Point(dropPosition.X, dropPosition.Y)
                };

                //var viewModel = (ConstructorVM)editor.DataContext;
                if (DataContext is ConstructorVM vm)
                {
                    vm.Nodes.Add(newNode);
                }
                    
            }
        }
        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listBox = sender as ListBox;
                if (listBox?.SelectedItem is NodeViewModel node)
                {
                    DragDrop.DoDragDrop(listBox, node, DragDropEffects.Copy);
                }
            }
        }
        private void ElementsListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ElementsListBox.SelectedItem is NodeViewModel selectedNode)
            {
                DragDrop.DoDragDrop(ElementsListBox, selectedNode, DragDropEffects.Copy);
            }
        }

        private void NodeInput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConnectorViewModel connector)
            {
                if (DataContext is ConstructorVM vm)
                {
                    vm.CompleteConnection(connector);
                }
            }
        }

        private void NodeOutput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ConnectorViewModel connector)
            {
                if (DataContext is ConstructorVM vm)
                {
                    vm.StartConnection(connector);
                }
            }
        }

        private void NodeInput_PendingConnectionStarted(object sender, Nodify.Events.PendingConnectionEventArgs e)
        {
            var test = 5;
        }

        private void NodeOutput_PendingConnectionCompleted(object sender, Nodify.Events.PendingConnectionEventArgs e)
        {
            if (DataContext is ConstructorVM vm)
            {
                var target = (ConnectorViewModel)e.TargetConnector;
                var connector = new ConnectorViewModel { Anchor = target.Anchor };
                vm.CompleteConnection(connector);
            }
        }

        private void NodeInput_PendingConnectionCompleted(object sender, Nodify.Events.PendingConnectionEventArgs e)
        {
            if (DataContext is ConstructorVM vm)
            {
                var connector = new ConnectorViewModel { Anchor = e.Anchor };
                vm.CompleteConnection(connector);
            }
        }

        private void NodeOutput_PendingConnectionStarted(object sender, Nodify.Events.PendingConnectionEventArgs e)
        {
            if (DataContext is ConstructorVM vm)
            {
                var connector = new ConnectorViewModel{Anchor = e.Anchor };
                vm.StartConnection(connector);
            }
        }
    }
}
