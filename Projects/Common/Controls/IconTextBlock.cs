using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class IconLabel : Label
	{
		static IconLabel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(IconLabel), new FrameworkPropertyMetadata(typeof(IconLabel)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(IconLabel));
		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconLabel));
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}