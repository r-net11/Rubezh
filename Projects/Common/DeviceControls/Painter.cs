using System;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace DeviceControls
{
	public class Painter : IPainter
	{
		public FrameworkElement Draw(ElementBase element)
		{
			var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == ((ElementDevice)element).DeviceUID);
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return DeviceControl.GetDefaultPicture(driverUID);
		}
	}
}