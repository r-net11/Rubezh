using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementPolyline : ElementBasePolyline, IElementZIndex, IPrimitive
	{
		public ElementPolyline()
		{
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementPolyline elementLine = new ElementPolyline()
			{
				BackgroundColor = BackgroundColor,
				BorderThickness = BorderThickness,
				Points = Points.Clone()
			};
			Copy(elementLine);
			return elementLine;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Polyline; }
		}

		#endregion
	}
}