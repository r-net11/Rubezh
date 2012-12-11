using System;
using System.Windows;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Controls;

namespace GKModule.Plans.Designer
{
	public class Painter : IPainter
	{
		public UIElement Draw(ElementBase element)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			var imageSource = DevicePictureCache.GetImageSource(device);
			return new Image()
			{
				Source = imageSource
			};
		}
	}
}