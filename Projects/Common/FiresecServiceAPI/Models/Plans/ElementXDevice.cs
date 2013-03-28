using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementXDevice : ElementBasePoint
	{
		public ElementXDevice()
		{
			XDeviceUID = Guid.Empty;
		}

		[DataMember]
		public Guid XDeviceUID { get; set; }

		public override ElementBase Clone()
		{
			ElementXDevice elementBase = new ElementXDevice()
			{
				XDeviceUID = XDeviceUID
			};
			Copy(elementBase);
			return elementBase;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 7;
		}
	}
}