using System;
using System.Windows;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace GKModule.Plans.Designer
{
	public class Painter : RectanglePainter
	{
		private Guid? _xdeviceUID = null;
		protected override Pen CreatePen(ElementBase element, Rect rect)
		{
			return null;
		}
		protected override Brush CreateBrush(ElementBase element, Rect rect)
		{
			_xdeviceUID = null;
			return new ImageBrush();
		}
		protected override void UpdateBrush(ElementBase element, Rect rect)
		{
			var elementXDevice = (ElementXDevice)element;
			if (_xdeviceUID != elementXDevice.XDeviceUID)
			{
				_xdeviceUID = elementXDevice.XDeviceUID;
				var device = Helper.GetXDevice(elementXDevice);
				ImageBrush.ImageSource = DevicePictureCache.GetImageSource(device);
			}
		}
	}
}