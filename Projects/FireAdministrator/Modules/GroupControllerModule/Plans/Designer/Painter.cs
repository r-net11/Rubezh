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
	public class Painter : PointPainter
	{
		private ElementXDevice _elementXDevice;
		public Painter(ElementXDevice elementXDevice)
			: base(elementXDevice)
		{
			_elementXDevice = elementXDevice;
		}

		protected override Brush GetBrush()
		{
			var xdevice = Helper.GetXDevice(_elementXDevice);
			return DevicePictureCache.GetXBrush(xdevice);
		}
	}
}