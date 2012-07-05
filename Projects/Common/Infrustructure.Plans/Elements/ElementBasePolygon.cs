using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolygon : ElementBaseShape
	{
		public ElementBasePolygon()
		{
			Type = ElementType.Polygon;
		}

		public override void SetDefault()
		{
			Points.Add(new Point(0, 0));
			Points.Add(new Point(50, 0));
			Points.Add(new Point(50, 50));
			Points.Add(new Point(0, 50));
		}
	}
}
