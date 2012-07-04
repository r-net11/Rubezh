using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolyline : ElementBaseShape
	{
		public ElementBasePolyline()
		{
			Type = ElementType.Polyline;
		}
	}
}
