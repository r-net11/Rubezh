using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class DirectionPropertiesView : UserControl
	{
		public DirectionPropertiesView()
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