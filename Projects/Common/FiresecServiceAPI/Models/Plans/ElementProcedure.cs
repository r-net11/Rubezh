using StrazhAPI.Plans.Elements;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class ElementProcedure : ElementBaseRectangle, IElementTextBlock, IPrimitive, IElementReference
	{
		public ElementProcedure()
		{
			BackgroundColor = Colors.Transparent;
			Text = "Процедура";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			FontFamilyName = "Arial";
			Height = 22;
			Width = 52;
		}

		[DataMember]
		public Guid ProcedureUID { get; set; }

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
			var elementBase = new ElementProcedure();
			Copy(elementBase);
			return elementBase;
		}

		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			var elementProcedure = (ElementProcedure)element;
			elementProcedure.ProcedureUID = ProcedureUID;
			elementProcedure.Text = Text;
			elementProcedure.ForegroundColor = ForegroundColor;
			elementProcedure.FontSize = FontSize;
			elementProcedure.FontItalic = FontItalic;
			elementProcedure.FontBold = FontBold;
			elementProcedure.FontFamilyName = FontFamilyName;
			elementProcedure.Stretch = Stretch;
			elementProcedure.TextAlignment = TextAlignment;
			elementProcedure.VerticalAlignment = VerticalAlignment;
			elementProcedure.WordWrap = WordWrap;
		}

		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return ProcedureUID; }
			set { ProcedureUID = value; }
		}

		#endregion IElementReference Members

		#region IPrimitive Members

		[XmlIgnore]
		public Primitive Primitive
		{
			get { return Primitive.TextBlock; }
		}

		#endregion IPrimitive Members
	}
}