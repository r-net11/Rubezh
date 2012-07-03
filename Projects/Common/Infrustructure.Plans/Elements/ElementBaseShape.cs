using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	[DataContract]
	public abstract class ElementBaseShape : ElementBase
	{
		public ElementBaseShape()
		{
			Points = new PointCollection();
		}

		[DataMember]
		public PointCollection Points { get; set; }

		protected virtual void Copy(ElementBaseShape element)
		{
			base.Copy(element);
			element.Points = Points.Clone();
		}
	}
}