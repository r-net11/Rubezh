using System.Windows.Controls;

namespace ClientFS2.Views
{
	public partial class ConfigurationDatabaseView : UserControl
	{
		public ConfigurationDatabaseView()
		{
			InitializeComponent();
			Loaded += new System.Windows.RoutedEventHandler(ConfigurationDatabaseView_Loaded);
		}

		void ConfigurationDatabaseView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
		}

		void On_SelectionChanged1(object sender, SelectionChangedEventArgs e)
		{
			DataGrid DataGrid1 = sender as DataGrid;
			if (DataGrid1.SelectedItem != null)
				DataGrid1.ScrollIntoView(DataGrid1.SelectedItem);
		}
	}
}