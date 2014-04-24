using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using SKDModule.Intervals.TimeTracking.ViewModels;

namespace SKDModule.Intervals.TimeTracking.Views
{
	/// <summary>
	/// Interaction logic for TimeTrackingView.xaml
	/// </summary>
	public partial class TimeTrackingView : UserControl
	{
		public TimeTrackingView()
		{
			InitializeComponent();
			Bind();
		}

		private void Bind()
		{
			var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
			if (dpd != null)
				dpd.AddValueChanged(grid, ItemSourceChanged);
		}
		private void ItemSourceChanged(object sender, EventArgs e)
		{
			var viewModel = (TimeTrackingViewModel)DataContext;
			var date = viewModel.FirstDay;
			for (int i = grid.Columns.Count - 1; i >= 5; i--)
				grid.Columns.RemoveAt(i);
			for (int i = 0; i < viewModel.TotalDays; i++)
			{
				DataGridTextColumn textColumn = new DataGridTextColumn();
				textColumn.Header = date.ToShortDateString();
				textColumn.Binding = new Binding(string.Format("Model.Hours[{0}]", i));
				grid.Columns.Add(textColumn);
				date = date.AddDays(1);
			}
		}
	}
}
