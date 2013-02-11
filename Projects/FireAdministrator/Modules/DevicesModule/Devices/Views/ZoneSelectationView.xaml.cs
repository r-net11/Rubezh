using System.Windows.Controls;

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
			ListView listView = sender as ListView;
			if (listView != null && listView.SelectedItem != null)
			{
				listView.ScrollIntoView(listView.SelectedItem);
			}
		}
    }
}