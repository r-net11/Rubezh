using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IElementZIndex, IPrimitive
	{
		public ElementRectangle()
		{
		}

		[DataMember]
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementRectangle elementBase = new ElementRectangle()
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
			get { return Infrustructure.Plans.Elements.Primitive.Rectangle; }
		}

		#endregion
	}
}