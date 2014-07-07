using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using SKDModule.ViewModels;
using System.Windows;

namespace SKDModule.Views
{
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
				//DataGridTextColumn textColumn = new DataGridTextColumn();
				//textColumn.Header = date.ToShortDateString();
				//textColumn.Binding = new Binding(string.Format("Model.Hours[{0}]", i));
				//grid.Columns.Add(textColumn);
				var factory = new FrameworkElementFactory(typeof(TextBox));
				factory.SetValue(TextBox.TextProperty, new Binding(string.Format("Model.EmployeeTimeTracks[{0}].Total", i)));
				var column = new DataGridTemplateColumn()
				{
					Header = date.ToShortDateString(),
					CellTemplate = new DataTemplate()
					{
						VisualTree = factory,
					},
				};
				grid.Columns.Add(column);
				date = date.AddDays(1);
			}
		}
	}
}