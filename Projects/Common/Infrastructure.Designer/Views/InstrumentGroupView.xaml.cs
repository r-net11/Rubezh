using System.Windows.Controls;

namespace Infrastructure.Designer.Views
{
    /// <summary>
    /// Interaction logic for InstrumentGroupView.xaml
    /// </summary>
    public partial class InstrumentGroupView : UserControl
    {
        public InstrumentGroupView()
        {
            InitializeComponent();
            ContextMenuOpening += InstrumentGroupView_ContextMenuOpening;
            ContextMenuClosing += InstrumentGroupView_ContextMenuClosing;
        }

        private void InstrumentGroupView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu.DataContext = DataContext;
        }
        private void InstrumentGroupView_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            ContextMenu.DataContext = null;
        }
    }
}
