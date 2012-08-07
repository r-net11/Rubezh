using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using System.Windows;
using Infrustructure.Plans.Elements;
using XFiresecAPI;
using FiresecAPI.Models;
using DeviceControls;
using FiresecClient;

namespace GKModule.Plans.Designer
{
	public class Painter : IPainter
	{
		public FrameworkElement Draw(ElementBase element)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return DeviceControl.GetDefaultPicture(driverUID);
		}
	}
}