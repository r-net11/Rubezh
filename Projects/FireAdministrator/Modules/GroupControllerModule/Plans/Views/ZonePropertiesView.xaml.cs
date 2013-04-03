using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class ZonePropertiesView : UserControl
	{
		public ZonePropertiesView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listView = sender as ListView;
			if (listView != null && listView.SelectedItem != null)
			{
				listView.ScrollIntoView(listView.SelectedItem);
			}
		}
	}
}