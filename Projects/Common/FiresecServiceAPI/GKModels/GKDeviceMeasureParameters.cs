using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDeviceMeasureParameters
	{
		public GKDeviceMeasureParameters()
		{
			MeasureParameterValues = new List<GKMeasureParameterValue>();
		}

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public List<GKMeasureParameterValue> MeasureParameterValues { get; set; }
	}
}