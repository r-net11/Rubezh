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
		public Guid AlternativeDriverUID { get; set; }

		public override ElementBase Clone()
		{
			ElementDevice elementBase = new ElementDevice()
			{
				DeviceUID = DeviceUID
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