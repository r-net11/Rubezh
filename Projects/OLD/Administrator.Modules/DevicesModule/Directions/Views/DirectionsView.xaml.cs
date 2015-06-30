using System.Windows;
using System.Windows.Controls;

namespace DevicesModule.Views
{
	public partial class DirectionsView : UserControl
	{
		public DirectionsView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(OnSelectionChanged);
			_directionDataGrid.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
		}

		void OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_directionDataGrid.SelectedItem != null)
				_directionDataGrid.ScrollIntoView(_directionDataGrid.SelectedItem);
		}
	}
}