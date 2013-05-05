using System.Windows.Controls;

namespace ClientFS2.Views
{
	public partial class ConfigurationDatabaseView : UserControl
	{
		public ConfigurationDatabaseView()
		{
			InitializeComponent();
		}

		void On_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid.SelectedItem != null)
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
		}
	}
}