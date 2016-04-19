using RubezhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IPrimitive
	{
		public ElementPolygon()
		{
			PresentationName = "Многоугольник";
		}

		[DataMember]
		public bool ShowTooltip { get; set; }

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return RubezhAPI.Plans.Elements.Primitive.Polygon; }
		}

		#endregion

	}
}