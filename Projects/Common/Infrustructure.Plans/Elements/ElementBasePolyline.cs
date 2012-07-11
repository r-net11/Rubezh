using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolyline : ElementBaseShape
	{
		public ElementBasePolyline()
		{
		}

		public override ElementType Type
		{
			get { return ElementType.Polyline; }
		}
		public override void SetDefault()
		{
			Points.Add(new Point(0, 0));
			Points.Add(new Point(100, 100));
		}
	}
}