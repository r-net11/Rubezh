using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
	public class UIExtension
	{
		public static readonly DependencyProperty ShowSelectionMarkProperty = DependencyProperty.RegisterAttached("ShowSelectionMark", typeof(bool), typeof(UIExtension), new PropertyMetadata(false, ShowSelectionMarkPropertyChanged));

		public static bool GetShowSelectionMark(DependencyObject obj)
		{
			return (bool)obj.GetValue(ShowSelectionMarkProperty);
		}
		public static void SetShowSelectionMark(DependencyObject obj, bool value)
		{
			obj.SetValue(ShowSelectionMarkProperty, value);
		}

		private static void ShowSelectionMarkPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var sender = obj as DataGrid;
		}

		private static T FindAncestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			current = VisualTreeHelper.GetParent(current);
			while (current != null)
			{
				if (current is T)
					return (T)current;
				current = VisualTreeHelper.GetParent(current);
			};
			return null;
		}
	}
}
