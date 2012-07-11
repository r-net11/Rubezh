using System.Windows;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using FiresecClient;
using System.Linq;
using System;
using System.Collections.Generic;

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