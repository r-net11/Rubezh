using Common;
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
	}
}