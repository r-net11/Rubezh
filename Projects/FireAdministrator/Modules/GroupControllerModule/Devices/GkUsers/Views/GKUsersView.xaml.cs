using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class GKUsersView : UserControl
	{
		public GKUsersView()
		{
			InitializeComponent();
		}

		private void dataGrid1RowSelected(object sender, RoutedEventArgs e)
		{
			DataGrid2.SelectedIndex = DataGrid1.SelectedIndex;
			DataGrid1.ScrollIntoView(DataGrid1.SelectedItem);
		}

		private void dataGrid2RowSelected(object sender, RoutedEventArgs e)
		{
			DataGrid1.SelectedIndex = DataGrid2.SelectedIndex;
			DataGrid2.ScrollIntoView(DataGrid2.SelectedItem);
		}

		private void DataGrid1_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			DataGridScroll(e, DataGrid1, DataGrid2);
		}

		private void DataGrid2_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			DataGridScroll(e, DataGrid2, DataGrid1);
		}

		void DataGridScroll(ScrollChangedEventArgs e, DataGrid dataGrid1, DataGrid dataGrid2)
		{
			if (e.HorizontalChange != 0.0f)
			{
				ScrollViewer sv;
				Type t = dataGrid1.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, dataGrid2, null) as ScrollViewer;
					sv.ScrollToHorizontalOffset(e.HorizontalOffset);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			if (e.VerticalChange != 0.0f)
			{
				ScrollViewer sv = null;
				Type t = dataGrid1.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, dataGrid2, null) as ScrollViewer;
					sv.ScrollToVerticalOffset(e.VerticalOffset);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}
	}
}