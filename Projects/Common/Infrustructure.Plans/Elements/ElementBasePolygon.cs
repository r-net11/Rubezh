﻿using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBasePolygon : ElementBaseShape
	{
		public ElementBasePolygon()
		{
		}

		[XmlIgnore]
		public override ElementType Type
		{
			get { return ElementType.Polygon; }
		}
	}
}