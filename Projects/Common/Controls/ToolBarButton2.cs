using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class ToolBarButton2 : Button
	{
		static ToolBarButton2()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarButton2), new FrameworkPropertyMetadata(typeof(ToolBarButton2)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(ToolBarButton2));
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ToolBarButton2));
		public static readonly DependencyProperty HasTextProperty = DependencyProperty.Register("HasText", typeof(bool), typeof(ToolBarButton2));

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

		public bool HasText
		{
			get { return (bool)GetValue(HasTextProperty); }
			set { SetValue(HasTextProperty, value); }
		}
	}
}