using System.Windows;
using System.Windows.Controls;

namespace PlansModule.Views
{
    public partial class PlansTree : UserControl
    {
        public PlansTree()
        {
            InitializeComponent();
        }

        private void PlansTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MessageBox.Show("Selected");
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

            
        }
    }
}
