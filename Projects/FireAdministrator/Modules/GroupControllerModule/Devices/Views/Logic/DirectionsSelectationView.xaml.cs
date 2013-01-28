using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DirectionsSelectationView : UserControl
	{
		public DirectionsSelectationView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListView listView = sender as ListView;
			if (listView != null && listView.SelectedItem != null)
			{
				listView.ScrollIntoView(listView.SelectedItem);
			}
		}
	}
}