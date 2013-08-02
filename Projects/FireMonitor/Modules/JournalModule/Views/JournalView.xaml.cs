using System.Windows.Controls;

namespace JournalModule.Views
{
	public partial class JournalView : UserControl
	{
		public JournalView()
		{
			InitializeComponent();
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (JournalDataGrid.SelectedItem != null)
				JournalDataGrid.ScrollIntoView(JournalDataGrid.SelectedItem);
		}
	}
}