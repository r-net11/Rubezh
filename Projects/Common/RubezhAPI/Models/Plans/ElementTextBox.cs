using Common;
using Infrustructure.Plans.Elements;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	public class ElementTextBox : ElementBaseRectangle, IElementTextBlock, IPrimitive
	{
		public ElementTextBox()
		{
			Stretch = false;
			TextAlignment = 0;
			VerticalAlignment = 0;
			WordWrap = false;
			BorderThickness = 0;
			BackgroundColor = Colors.Transparent;
			PresentationName = "Текст";
			Text = "Надпись";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			FontItalic = false;
			FontBold = false;
			Height = 22;
			Width = 52;
		}

		public string Text { get; set; }
		public Color ForegroundColor { get; set; }
		public double FontSize { get; set; }
		public bool FontItalic { get; set; }
		public bool FontBold { get; set; }
		public string FontFamilyName { get; set; }
		public bool Stretch { get; set; }
		public int TextAlignment { get; set; }
		public int VerticalAlignment { get; set; }
		public bool WordWrap { get; set; }
		public bool ShowTooltip { get; set; }

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.TextBox; }
		}
	}
}