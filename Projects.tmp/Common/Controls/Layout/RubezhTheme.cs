using System;
using Xceed.Wpf.AvalonDock.Themes;

namespace Controls.Layout
{
	public class RubezhTheme : Theme
	{
		public override Uri GetResourceUri()
		{
			return new Uri("/Controls;component/Layout/Theme.xaml", UriKind.Relative);
		}
	}
}
