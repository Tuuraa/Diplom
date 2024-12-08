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

namespace WPFComponents.View
{
    /// <summary>
    /// Логика взаимодействия для ScenarioBuilder.xaml
    /// </summary>
    public partial class ScenarioBuilder : Window
    {
        public ScenarioBuilder()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var button = sender as Button;
                if (button != null)
                {
                    DragDrop.DoDragDrop(button, button.Content, DragDropEffects.Copy);
                }
            }
        }

        private void ScenarioCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string commandName = e.Data.GetData(DataFormats.StringFormat).ToString();
                var commandBlock = new CommandBlockUC(commandName);

                var canvas = sender as Canvas;
                Point dropPosition = e.GetPosition(canvas);
                if (commandBlock != null)
                {
                    
                    if (canvas != null && !canvas.Children.Contains(commandBlock))
                    {
                        Canvas.SetLeft(commandBlock, dropPosition.X);
                        Canvas.SetTop(commandBlock, dropPosition.Y);
                        canvas.Children.Add(commandBlock); // Добавляем блок в Canvas, если его там ещё нет
                    }
                }
               
                dropPosition = e.GetPosition(ScenarioCanvas);
                Canvas.SetLeft(commandBlock, dropPosition.X);
                Canvas.SetTop(commandBlock, dropPosition.Y);
            }
        }
        private void ScenarioCanvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

    }
}
