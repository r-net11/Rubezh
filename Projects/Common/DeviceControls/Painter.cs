using System.Windows;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace DeviceControls
{
	public class Painter : IPainter
	{
		public FrameworkElement Draw(ElementBase element)
		{
			return DeviceControl.GetDefaultPicture(((ElementDevice)element).Device.Driver.UID);
		}
	}
}