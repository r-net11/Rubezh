using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XDeviceMeasureParameters
	{
		public XDeviceMeasureParameters()
		{
			MeasureParameterValues = new List<XMeasureParameterValue>();
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public List<XMeasureParameterValue> MeasureParameterValues { get; set; }
	}
}