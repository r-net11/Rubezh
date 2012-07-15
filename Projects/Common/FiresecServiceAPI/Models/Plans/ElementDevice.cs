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

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementDevice()
			{
				DeviceUID = DeviceUID
			};
			Copy(elementBase);
			return elementBase;
		}
	}
}