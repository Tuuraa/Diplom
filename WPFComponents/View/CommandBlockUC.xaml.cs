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
    /// Логика взаимодействия для CommandBlockUC.xaml
    /// </summary>
    public partial class CommandBlockUC : UserControl
    {
        public string CommandName { get; set; }
        private bool isDragging = false;
        private Point clickPosition;

        public Point TopConnector => new Point(Canvas.GetLeft(this) + Width / 2, Canvas.GetTop(this)); // Верхняя точка стыковки
        public Point BottomConnector => new Point(Canvas.GetLeft(this) + Width / 2, Canvas.GetTop(this) + Height); // Нижняя точка стыковки


        public CommandBlockUC(string commandName)
        {
            InitializeComponent();
            CommandName = commandName;
            DataContext = this;

            this.MouseDown += CommandBlock_MouseDown;
            this.MouseMove += CommandBlock_MouseMove;
            this.MouseUp += CommandBlock_MouseUp;
        }
        private void CommandBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                DragDrop.DoDragDrop(this, new DataObject(typeof(CommandBlockUC), this), DragDropEffects.Move);
                clickPosition = e.GetPosition(this);
                this.CaptureMouse();
            }
        }

        private void CommandBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var parent = this.Parent as Canvas;
                if (parent != null)
                {
                    Point newPosition = e.GetPosition(parent);
                    Canvas.SetLeft(this, newPosition.X - clickPosition.X);
                    Canvas.SetTop(this, newPosition.Y - clickPosition.Y);
                }
            }
        }

        private void CommandBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                this.ReleaseMouseCapture();
            }
        }

        private bool IsCloseToConnector(Point p1, Point p2, double threshold = 20)
        {
            return Math.Abs(p1.X - p2.X) < threshold && Math.Abs(p1.Y - p2.Y) < threshold;
        }

        private void SnapTo(CommandBlockUC otherBlock)
        {
            Canvas.SetLeft(this, Canvas.GetLeft(otherBlock));
            Canvas.SetTop(this, Canvas.GetTop(otherBlock) + otherBlock.Height);
        }
    }
}
