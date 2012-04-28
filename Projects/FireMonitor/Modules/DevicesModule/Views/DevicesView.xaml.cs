using System.Windows.Controls;

namespace DevicesModule.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
		{
			InitializeComponent();
			_devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(_devicesDataGrid_SelectionChanged);
		}

		void _devicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_devicesDataGrid.SelectedItem != null)
				_devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
		}
	}
}