using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using StrazhAPI.SKD;
using Infrastructure.Common.Services;
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
				dpd.AddValueChanged(Grid, ItemSourceChanged);
		}

		private void ItemSourceChanged(object sender, EventArgs e)
		{
			var viewModel = (TimeTrackingViewModel)DataContext;
			var date = viewModel.FirstDay;
			for (int i = Grid.Columns.Count - 1; i >= 3; i--)
				Grid.Columns.RemoveAt(i);
			for (int i = 0; i < viewModel.TotalDays; i++)
			{
				var factory = new FrameworkElementFactory(typeof(TimeTrackingDayControlView)); //TODO: Remove obsolete realization
				factory.SetValue(DataContextProperty, new Binding(string.Format("DayTracks[{0}]", i)));

				var column = new DataGridTemplateColumn
				{
					Width = 60,
					CanUserResize = false,
					CanUserSort = false,
					CellTemplate = new DataTemplate
					{
						VisualTree = factory,
					},
				};

				var holiday = viewModel.HolydaysOfCurrentOrganisation.FirstOrDefault(x => x.Date.Day == date.Day && x.Date.Month == date.Month);

				column.Header = CreateColumnHeader(date, holiday);

				Grid.Columns.Add(column);
				date = date.AddDays(1);
			}
		}

		private TextBlock CreateColumnHeader(DateTime date, Holiday holiday)
		{
			return new TextBlock
			{
				Text = date.ToString("dd MM"),
				ToolTip = holiday != null ? holiday.Name : date.ToString("dddd", new CultureInfo("ru-RU")),
				Foreground = GetForegroundBrush(holiday, date),
				HorizontalAlignment = HorizontalAlignment.Center,
				FontWeight = FontWeights.Bold
			};
		}

		private Brush GetForegroundBrush(Holiday holiday, DateTime date)
		{
			if (holiday != null)
				return Brushes.Khaki;

			if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
				return Brushes.DarkGray;

			return Brushes.WhiteSmoke;
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