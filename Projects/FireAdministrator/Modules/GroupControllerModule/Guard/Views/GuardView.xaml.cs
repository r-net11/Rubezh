using System.Windows.Controls;
using System.Windows;

namespace GKModule.Views
{
	public partial class GuardView : UserControl
	{
		public GuardView()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(OnSelectionChanged);
			_usersDataGrid.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
		}

		void OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_usersDataGrid.SelectedItem != null)
				_usersDataGrid.ScrollIntoView(_usersDataGrid.SelectedItem);
		}
	}
}