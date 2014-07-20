using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartSKDVerificationProperties : ILayoutProperties
	{
		[DataMember]
		public Guid ReaderDeviceUID { get; set; }
	}
}