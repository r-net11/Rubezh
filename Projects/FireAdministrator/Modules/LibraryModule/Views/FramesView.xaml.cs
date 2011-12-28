using System.Windows;
using System.Windows.Controls;

namespace LibraryModule.Views
{
    public partial class FramesView : UserControl
    {
        public FramesView()
        {
            InitializeComponent();
        }

        void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            frameworkElement.ContextMenu.DataContext = DataContext;
        }
    }
}