using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Controls
{
	public class TextBoxHelper
	{
		public static readonly DependencyProperty HighlightTextOnFocusProperty = DependencyProperty.RegisterAttached("HighlightTextOnFocus", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, HighlightTextOnFocusPropertyChanged));

		[AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static bool GetHighlightTextOnFocus(DependencyObject obj)
		{
			return (bool)obj.GetValue(HighlightTextOnFocusProperty);
		}
		public static void SetHighlightTextOnFocus(DependencyObject obj, bool value)
		{
			obj.SetValue(HighlightTextOnFocusProperty, value);
		}

		private static void HighlightTextOnFocusPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var sender = obj as UIElement;
			if (obj != null)
			{
				if ((bool)e.NewValue)
				{
					sender.GotFocus += new RoutedEventHandler(OnGotFocus);
					sender.PreviewMouseLeftButtonDown += OnMouseLeftButtonDownSetFocus;
				}
				else
				{
					sender.GotFocus -= new RoutedEventHandler(OnGotFocus);
					sender.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDownSetFocus;
				}
			}
		}

		static void OnGotFocus(object sender, RoutedEventArgs e)
		{
			var textBox = e.OriginalSource as TextBox;
			if (textBox != null)
			{
				textBox.SelectAll();
				System.Diagnostics.Trace.WriteLine("sender_GotFocus");
			}
		}
		private static void OnMouseLeftButtonDownSetFocus(object sender, MouseButtonEventArgs e)
		{
			TextBox tb = FindAncestor<TextBox>((DependencyObject)e.OriginalSource);
			if (tb == null)
				return;
			if (!tb.IsKeyboardFocusWithin)
			{
				tb.Focus();
				e.Handled = true;
			}
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