using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolygon : ElementBasePolygon, IElementZIndex, IPrimitive
	{
		public ElementPolygon()
		{
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementPolygon();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polygon; }
		}

		#endregion
	}
}