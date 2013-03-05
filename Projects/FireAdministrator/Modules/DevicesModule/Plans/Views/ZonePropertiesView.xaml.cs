using System.Windows.Controls;
using System.Linq;

namespace DevicesModule.Plans.Views
{
    public partial class ZonePropertiesView : UserControl
    {
        public ZonePropertiesView()
        {
            InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(ZonePropertiesView_Loaded);
        }

		void ZonePropertiesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (ZonesDataGrid.SelectedItem == null)
			{
				if (ZonesDataGrid.Items.Count > 0)
				{
					var lastItem = ZonesDataGrid.Items[ZonesDataGrid.Items.Count - 1];
					ZonesDataGrid.ScrollIntoView(lastItem);
				}
			}
		}

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }
    }
}