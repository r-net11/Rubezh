using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class HeaderedScrollViewer : ScrollViewer
	{
		public static DependencyProperty ScrollViewerHeaderProperty = DependencyProperty.Register("ScrollViewerHeader", typeof(FrameworkElement), typeof(HeaderedScrollViewer), new FrameworkPropertyMetadata(null));
		public FrameworkElement ScrollViewerHeader
		{
			get { return (FrameworkElement)GetValue(ScrollViewerHeaderProperty); }
			set { SetValue(ScrollViewerHeaderProperty, value); }
		}
	}
}
