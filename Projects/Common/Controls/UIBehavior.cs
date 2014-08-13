using System.Windows;

namespace Controls
{
	public class UIBehavior
	{
		public static readonly DependencyProperty ShowSelectionMarkProperty = DependencyProperty.RegisterAttached("ShowSelectionMark", typeof(bool), typeof(UIBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

		public static bool GetShowSelectionMark(DependencyObject obj)
		{
			return (bool)obj.GetValue(ShowSelectionMarkProperty);
		}
		public static void SetShowSelectionMark(DependencyObject obj, bool value)
		{
			obj.SetValue(ShowSelectionMarkProperty, value);
		}
	}
}
