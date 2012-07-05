using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangle : ElementBaseRectangle, IElementZIndex
	{
		public ElementRectangle()
		{
			BackgroundColor = Colors.White;
			BorderColor = Colors.Black;
			BorderThickness = 1;
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
	}
}