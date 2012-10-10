using System.Windows.Controls;
using DevicesModule.ViewModels;

namespace DevicesModule.Views
{
    public partial class ZoneSelectationView : UserControl
    {
        public ZoneSelectationView()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                listView.ScrollIntoView(listView.SelectedItem);
            }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                var zoneSelectationViewModel = DataContext as ZoneSelectationViewModel;
                if (zoneSelectationViewModel != null)
                {
                    if (listView.SelectedItem != null)
                    {
                        zoneSelectationViewModel.SaveCommand.Execute();
                    }
                }
            }
        }
    }
}