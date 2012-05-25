using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevicesModule.Views
{
	public partial class DevicesView : UserControl
	{
		public DevicesView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(DevicesView_Loaded);
			_devicesDataGrid.SelectionChanged += new SelectionChangedEventHandler(DevicesView_SelectionChanged);
		}

		void DevicesView_Loaded(object sender, RoutedEventArgs e)
		{
			if (_devicesDataGrid.SelectedItem != null)
				_devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
			Focus();

			_grid1.Focus();
		}

		void DevicesView_SelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_devicesDataGrid.SelectedItem != null)
				_devicesDataGrid.ScrollIntoView(_devicesDataGrid.SelectedItem);
		}
	}
}