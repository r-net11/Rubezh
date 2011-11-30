using System.Windows.Controls;
using System.Windows;

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