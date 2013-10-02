using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GKModule.Views
{
	public partial class DeviceConfigurationView : UserControl
	{
		public DeviceConfigurationView()
		{
			InitializeComponent();
		}

		private void dataGrid1RowSelected(object sender, RoutedEventArgs e)
		{
			DataGrid2.SelectedIndex = DataGrid1.SelectedIndex;
		}

		private void dataGrid2RowSelected(object sender, RoutedEventArgs e)
		{
			DataGrid1.SelectedIndex = DataGrid2.SelectedIndex;
		}

		private void DataGrid1_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.HorizontalChange != 0.0f)
			{
				ScrollViewer sv = null;
				Type t = DataGrid1.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, DataGrid2, null) as ScrollViewer;
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
				Type t = DataGrid1.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, DataGrid2, null) as ScrollViewer;
					sv.ScrollToVerticalOffset(e.VerticalOffset);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void DataGrid2_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.HorizontalChange != 0.0f)
			{
				ScrollViewer sv = null;
				Type t = DataGrid2.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, DataGrid1, null) as ScrollViewer;
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
				Type t = DataGrid2.GetType();
				try
				{
					sv = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, DataGrid1, null) as ScrollViewer;
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