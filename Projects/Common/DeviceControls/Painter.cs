using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Painters;
using System.Windows;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;

namespace DeviceControls
{
	public class Painter : IPainter
	{
		#region IPainter Members

		public FrameworkElement Draw(ElementBase element)
		{
			return DeviceControl.GetDefaultPicture(((ElementDevice)element).Device.Driver.UID);
		}

		#endregion
	}
}
