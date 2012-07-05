using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementEllipse : ElementBaseRectangle, IElementZIndex
	{
		public ElementEllipse()
		{
			BackgroundColor = Colors.White;
			BorderColor = Colors.Black;
			BorderThickness = 1;
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
	}
}