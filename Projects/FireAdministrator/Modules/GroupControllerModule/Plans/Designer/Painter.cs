using System;
using System.Windows;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace GKModule.Plans.Designer
{
	public class Painter : IPainter
	{
		public System.Windows.Media.Visual Draw(ElementBase element)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return DeviceControl.GetDefaultPicture(driverUID);
		}
	}
}