using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Controls.Behaviours
{
	public static class DatePickerCustomWatermark
	{
		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.RegisterAttached(
				"Watermark",
				typeof(Brush),
				typeof(DatePickerCustomWatermark),
				new UIPropertyMetadata(null, OnWatermarkChanged));

		public static readonly DependencyProperty WatermarkTextProperty =
			DependencyProperty.RegisterAttached(
				"WatermarkText",
				typeof(string),
				typeof(DatePickerCustomWatermark),
				new UIPropertyMetadata(null, OnWatermarkChanged));

		public static string GetWatermarkText(Control control)
		{
			return (string)control.GetValue(WatermarkTextProperty);
		}

		public static void SetWatermarkText(Control control, string value)
		{
			control.SetValue(WatermarkTextProperty, value);
		}

		public static Brush GetWatermark(Control control)
		{
			return (Brush)control.GetValue(WatermarkProperty);
		}

		public static void SetWatermark(Control control, Brush value)
		{
			control.SetValue(WatermarkProperty, value);
		}

		private static void OnWatermarkChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var datePicker = dependencyObject as DatePicker;
			if (datePicker == null)
				return;

			if ((e.NewValue != null) && (e.OldValue == null))
				datePicker.Loaded += DatePickerLoaded;
			else if ((e.NewValue == null) && (e.OldValue != null))
				datePicker.Loaded -= DatePickerLoaded;
		}

		private static void DatePickerLoaded(object sender, RoutedEventArgs e)
		{
			var datePicker = sender as DatePicker;
			if (datePicker == null)
				return;

			var datePickerTextBox = GetFirstChildOfType<DatePickerTextBox>(datePicker);
			if (datePickerTextBox == null)
				return;

			if(datePickerTextBox.Template == null) return;
			var partWatermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
			if (partWatermark == null)
				return;

			partWatermark.Foreground = GetWatermark(datePicker);
			partWatermark.Content = GetWatermarkText(datePicker);
		}

		private static T GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
		{
			if (dependencyObject == null)
				return null;

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
			{
				var child = VisualTreeHelper.GetChild(dependencyObject, i);
				var result = (child as T) ?? GetFirstChildOfType<T>(child);
				if (result != null)
					return result;
			}

			return null;
		}
	}
}
