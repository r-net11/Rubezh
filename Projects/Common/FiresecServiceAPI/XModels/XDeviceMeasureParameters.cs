using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
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