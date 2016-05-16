using System.Runtime.Serialization;

namespace RubezhAPI.Plans.Elements
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

		public override ElementBase Clone()
		{
			ElementBaseShape newElement = (ElementBaseShape)base.Clone();
			newElement.Points = new PointCollection(newElement.Points);
			return newElement;
		}
	}
}