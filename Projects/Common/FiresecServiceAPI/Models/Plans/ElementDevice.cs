using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementDevice : ElementBasePoint
	{
		public ElementDevice()
		{
			DeviceUID = Guid.Empty;
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public Guid AlternativeLibraryDeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementDevice elementBase = new ElementDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementDevice)element).DeviceUID = DeviceUID;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}