using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace WPFComponents.Utils
{
    internal class AgreeBalloon : UIElement
    {
        private Border _border;
        private Button _agreeButton;

        public event RoutedEventHandler AgreeClicked;

        public AgreeBalloon(string message)
        {
            // Создание контейнера для содержимого баллона
            _border = new Border
            {
                Background = Brushes.LightYellow,
                Width = 300,
                Height = 120,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5)
            };

            var grid = new Grid();

            // Добавление текста
            var textBlock = new TextBlock
            {
                Text = message,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 20, 10, 10),
                FontSize = 14
            };

            // Создание кнопки "Согласен"
            _agreeButton = new Button
            {
                Content = "Согласен",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 100,
                Height = 30,
                Margin = new Thickness(0, 10, 0, 10)
            };
            _agreeButton.Click += OnAgreeButtonClick;

            grid.Children.Add(textBlock);
            grid.Children.Add(_agreeButton);
            _border.Child = grid;

            // Добавление границы в элемент
            this.AddVisualChild(_border);
        }

        // Переопределение VisualChildrenCount и GetVisualChild для корректной работы с VisualTree
        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _border;
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        // Метод для обработки нажатия кнопки "Согласен"
        private void OnAgreeButtonClick(object sender, RoutedEventArgs e)
        {
            AgreeClicked?.Invoke(this, e);
        }
    }
}
