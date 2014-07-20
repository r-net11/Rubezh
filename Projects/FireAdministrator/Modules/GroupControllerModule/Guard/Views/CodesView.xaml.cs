using System.Windows;
using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class CodesView : UserControl
	{
		public CodesView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(OnSelectionChanged);
			_codesDataGrid.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
		}

		void OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_codesDataGrid.SelectedItem != null)
				_codesDataGrid.ScrollIntoView(_codesDataGrid.SelectedItem);
		}
	}
}