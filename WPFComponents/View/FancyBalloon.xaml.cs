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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFComponents.View
{
    /// <summary>
    /// Логика взаимодействия для FancyBalloon.xaml
    /// </summary>
    public partial class FancyBalloon : UserControl
    {
        // Событие для уведомления, что пользователь нажал кнопку
        public event RoutedEventHandler AgreeClicked;

        public FancyBalloon(string message)
        {
            InitializeComponent();

            // Устанавливаем текст сообщения
            MessageTextBlock.Text = message;
        }

        // Обработчик нажатия на кнопку "Согласен"
        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            AgreeClicked?.Invoke(this, e);
        }
    }

}
