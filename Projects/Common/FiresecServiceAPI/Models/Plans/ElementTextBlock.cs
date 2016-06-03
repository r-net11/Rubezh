using StrazhAPI.Plans.Elements;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementTextBlock : ElementBaseRectangle, IElementTextBlock, IPrimitive
	{
		public ElementTextBlock()
		{
			BackgroundColor = Colors.Transparent;
			PresentationName = "Текст";
			Text = "Надпись";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			FontFamilyName = "Arial";
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
			var elementBase = new ElementTextBlock();
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
			get { return Primitive.TextBlock; }
		}

		#endregion IPrimitive Members
	}
}