using System.Windows.Controls;

namespace GKModule.Plans.Views
{
	public partial class XZonePropertiesView : UserControl
	{
		public XZonePropertiesView()
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
	}
}