using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class ToolBarButton : Button
	{
		static ToolBarButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarButton), new FrameworkPropertyMetadata(typeof(ToolBarButton)));
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(ToolBarButton));

		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public ToolBarButton()
		{
			//Image image;image.Source
		}
	}
}
