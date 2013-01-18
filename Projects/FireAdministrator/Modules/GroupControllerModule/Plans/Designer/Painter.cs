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
		protected override Pen CreatePen(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override SolidColorBrush CreateSolidColorBrush(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override void UpdateImageBrush(ElementBase element, Rect rect)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			ImageBrush.ImageSource = DevicePictureCache.GetImageSource(device);
		}
	}
}