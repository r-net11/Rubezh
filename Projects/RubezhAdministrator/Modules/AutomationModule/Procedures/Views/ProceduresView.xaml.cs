using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutomationModule.Views
{
	public partial class ProceduresView : UserControl
	{
		public ProceduresView()
		{
			InitializeComponent();
		}

		private void StepsView_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			var element = sender as UIElement;
			if (element != null)
			{
				element.Focusable = true;
				Keyboard.Focus(element);
			}
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGrid dataGrid = sender as DataGrid;
			if (dataGrid != null && dataGrid.SelectedItem != null && !dataGrid.IsMouseOver)
			{
				dataGrid.ScrollIntoView(dataGrid.SelectedItem);
			}
		}
	}
}