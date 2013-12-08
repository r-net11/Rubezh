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
			MeasureParameters = new List<XMeasureParameter>();
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public List<XMeasureParameter> MeasureParameters { get; set; }
	}
}