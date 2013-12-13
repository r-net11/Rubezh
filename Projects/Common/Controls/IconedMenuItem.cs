using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class IconedMenuItem : MenuItem
	{
		static IconedMenuItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(IconedMenuItem), new FrameworkPropertyMetadata(typeof(IconedMenuItem)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(IconedMenuItem));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconedMenuItem));
		
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
