using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public class RGPointCollection
	{
		public RGPointCollection()
		{
			Points = new List<RGPoint>();
		}

		[DataMember]
		public List<RGPoint> Points { get; set; }
	}

	[DataContract]
	public class RGPoint
	{
		[DataMember]
		public double X { get; set; }

		[DataMember]
		public double Y { get; set; }
	}
}