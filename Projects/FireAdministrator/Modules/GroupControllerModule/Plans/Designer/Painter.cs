using System;
using System.Windows;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecClient;

namespace GKModule.Plans.Designer
{
	public class Painter : RectanglePainter
	{
		protected override void InitializeBrushes(ElementBase element, Rect rect)
		{
			base.InitializeBrushes(element, rect);
			SolidColorBrush.Color = Colors.Transparent;
			SolidColorBrush.Freeze();
		}
		protected override void UpdateImageBrush(ElementBase element, Rect rect)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			ImageBrush.ImageSource = DevicePictureCache.GetImageSource(device);
		}
	}
}