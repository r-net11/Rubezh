using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementSKDDevice : ElementBasePoint
	{
		public ElementSKDDevice()
		{
			DeviceUID = Guid.Empty;
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementSKDDevice elementBase = new ElementSKDDevice();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementSKDDevice)element).DeviceUID = DeviceUID;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}
	}
}