using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SKDModule.ViewModels;

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
			for (int i = grid.Columns.Count - 1; i >= 3; i--)
				grid.Columns.RemoveAt(i);
			for (int i = 0; i < viewModel.TotalDays; i++)
			{
				var factory = new FrameworkElementFactory(typeof(TimeTrackingCellView));
				factory.SetValue(TimeTrackingCellView.DataContextProperty, new Binding(string.Format("DayTracks[{0}]", i)));

				var column = new DataGridTemplateColumn()
				{
					Width = 60,
					CanUserResize = false,
					CanUserSort = false,
					CellTemplate = new DataTemplate()
					{
						VisualTree = factory,
					},
				};

				var textBlock = new TextBlock();
				textBlock.Text = date.ToString("dd MM");
				textBlock.ToolTip = DayOfWeekToString(date.DayOfWeek);
				if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
				{
					textBlock.Foreground = Brushes.DarkGray;
				}
				textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				textBlock.FontWeight = FontWeights.Bold;
				column.Header = textBlock;

				grid.Columns.Add(column);
				date = date.AddDays(1);
			}
		}

		string DayOfWeekToString(DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
				case DayOfWeek.Sunday:
					return "Воскресенье";
				case DayOfWeek.Monday:
					return "Понедельник";
				case DayOfWeek.Tuesday:
					return "Вторник";
				case DayOfWeek.Wednesday:
					return "Среда";
				case DayOfWeek.Thursday:
					return "Четверг";
				case DayOfWeek.Friday:
					return "Пятница";
				case DayOfWeek.Saturday:
					return "Суббота";
			}
			return "";
		}
	}

	public static class VirtualizingStackPanelBehaviors
	{
		public static bool GetIsPixelBasedScrolling(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsPixelBasedScrollingProperty);
		}

		public static void SetIsPixelBasedScrolling(DependencyObject obj, bool value)
		{
			obj.SetValue(IsPixelBasedScrollingProperty, value);
		}

		public static readonly DependencyProperty IsPixelBasedScrollingProperty =
			DependencyProperty.RegisterAttached("IsPixelBasedScrolling", typeof(bool), typeof(VirtualizingStackPanelBehaviors), new UIPropertyMetadata(false, OnIsPixelBasedScrollingChanged));

		private static void OnIsPixelBasedScrollingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var virtualizingStackPanel = o as VirtualizingStackPanel;
			if (virtualizingStackPanel == null)
				throw new InvalidOperationException();

			var isPixelBasedPropertyInfo = typeof(VirtualizingStackPanel).GetProperty("IsPixelBased", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
			if (isPixelBasedPropertyInfo == null)
				throw new InvalidOperationException();

			isPixelBasedPropertyInfo.SetValue(virtualizingStackPanel, (bool)(e.NewValue), null);
		}
	}
}