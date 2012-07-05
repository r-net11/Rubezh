using System;
using System.Runtime.Serialization;
using System.Windows;
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

		public Device Device { get; set; }
		public DeviceState DeviceState { get; set; }

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