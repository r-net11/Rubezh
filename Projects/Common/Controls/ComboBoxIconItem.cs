using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class ComboBoxIconItem : ComboBoxItem
	{
		static ComboBoxIconItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxIconItem), new FrameworkPropertyMetadata(typeof(ComboBoxIconItem)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(ComboBoxIconItem));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ComboBoxIconItem));
		
		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
