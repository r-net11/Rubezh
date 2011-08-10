using System.Windows;
using System.Windows.Controls;
using PlansModule.ViewModels;
namespace PlansModule.Views
{
    public partial class PlansTree : UserControl
    {
        public PlansTree()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            if ((treeView.SelectedItem as PlanDetailsViewModel) != null)
            {
                (DataContext as PlansViewModel).SelectedPlan =
                    treeView.SelectedItem as PlanDetailsViewModel;
            }
        }

        private void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            var treeView = sender as TreeView;
            if (((treeView.SelectedItem as PlanDetailsViewModel) != null)&&(treeView.SelectedItem!=null))
            {
                (DataContext as PlansViewModel).SelectedPlan = null;
            }
            
        }
    }
}
