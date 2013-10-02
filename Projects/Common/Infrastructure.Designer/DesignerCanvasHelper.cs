using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace Infrastructure.Designer
{
	public static class DesignerCanvasHelper
	{
		public static StackPanel BuildMenuHeader(string title, string imageSourceUri)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = title;
			textBlock.VerticalAlignment = VerticalAlignment.Center;

			Image image = new Image();
			image.Width = 16;
			image.VerticalAlignment = VerticalAlignment.Center;
			BitmapImage sourceImage = new BitmapImage();
			sourceImage.BeginInit();
			sourceImage.UriSource = new Uri(imageSourceUri);
			sourceImage.EndInit();
			image.Source = sourceImage;

			StackPanel stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Children.Add(image);
			stackPanel.Children.Add(textBlock);

			return stackPanel;
		}
	}
}