using System.Windows.Controls;

namespace FireMonitor.Views
{
	public partial class ProgressView : UserControl
	{
		public ProgressView()
		{
			InitializeComponent();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
				if(listBox != null)
				{
				}
		}
	}
}