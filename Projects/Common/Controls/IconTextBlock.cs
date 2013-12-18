using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class IconTextBlock : Label
	{
		static IconTextBlock()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTextBlock), new FrameworkPropertyMetadata(typeof(IconTextBlock)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(IconTextBlock));
		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconTextBlock));
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
