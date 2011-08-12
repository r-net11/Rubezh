using System.Windows;
using System.Windows.Controls;
using PlansModule.ViewModels;
using System.Windows.Input;

namespace PlansModule.Views
{
    public partial class PlansTree : UserControl
    {
        public PlansTree()
        {
            InitializeComponent();
        }

        private void TreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var element = Mouse.DirectlyOver;
            var treeView = sender as TreeView;
            if (!(element is TextBlock))
            {
                (DataContext as PlansViewModel).SelectedPlan = null;
            }
            else
            {
                (DataContext as PlansViewModel).SelectedPlan =    treeView.SelectedItem as PlanViewModel;
            }
        }
    }
}
