using System.Windows.Controls;

namespace LibraryModule.Views
{
    public partial class FramesView : UserControl
    {
        public FramesView()
        {
            InitializeComponent();
        }

        void TabControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            (Resources["tabItemContextMenu"] as ContextMenu).DataContext = DataContext;
        }
    }
}