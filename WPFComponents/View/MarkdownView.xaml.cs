using System.Windows;
using WPFComponents.Model;

namespace WPFComponents.View
{
    public partial class MarkdownView : Window
    {
        public MarkdownView()
        {
            InitializeComponent();

            MarkdownViewer.NavigateToString(MarkdownService.TestMarkdownShow());
        }
    }
}
