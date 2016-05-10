using Infrustructure.Plans.Elements;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementTextBlock : ElementBaseRectangle, IElementTextBlock, IPrimitive
	{
		public ElementTextBlock()
		{
			Stretch = false;
			TextAlignment = 0;
			VerticalAlignment = 0;
			WordWrap = false;
			BorderThickness = 0;
			BackgroundColor = Colors.Transparent;
            PresentationName = Resources.Language.Models.Plans.ElementTextBlock.PresentationName;
            Text = Resources.Language.Models.Plans.ElementTextBlock.Text;
			ForegroundColor = Colors.Black;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			FontItalic = false;
			FontBold = false;
			Height = 22;
			Width = 52;
		}

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public Color ForegroundColor { get; set; }

		[DataMember]
		public double FontSize { get; set; }

		[DataMember]
		public bool FontItalic { get; set; }

		[DataMember]
		public bool FontBold { get; set; }

		[DataMember]
		public string FontFamilyName { get; set; }

		[DataMember]
		public bool Stretch { get; set; }

		[DataMember]
		public int TextAlignment { get; set; }

		[DataMember]
		public int VerticalAlignment { get; set; }

		[DataMember]
		public bool WordWrap { get; set; }

		public override ElementBase Clone()
		{
			ElementTextBlock elementBase = new ElementTextBlock();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			var elementTextBlock = (ElementTextBlock)element;
			elementTextBlock.Text = Text;
			elementTextBlock.ForegroundColor = ForegroundColor;
			elementTextBlock.FontSize = FontSize;
			elementTextBlock.FontFamilyName = FontFamilyName;
			elementTextBlock.Stretch = Stretch;
			elementTextBlock.TextAlignment = TextAlignment;
			elementTextBlock.FontBold = FontBold;
			elementTextBlock.FontItalic = FontItalic;
		}

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.TextBlock; }
		}

		#endregion IPrimitive Members
	}
}