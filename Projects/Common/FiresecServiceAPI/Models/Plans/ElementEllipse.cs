using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IElementZIndex, IPrimitive
	{
		public ElementEllipse()
		{
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementEllipse elementBase = new ElementEllipse()
			{
				BackgroundColor = BackgroundColor,
				BorderColor = BorderColor,
				BorderThickness = BorderThickness
			};
			if (BackgroundPixels != null)
				elementBase.BackgroundPixels = (byte[])BackgroundPixels.Clone();
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.Ellipse; }
		}

		#endregion
	}
}