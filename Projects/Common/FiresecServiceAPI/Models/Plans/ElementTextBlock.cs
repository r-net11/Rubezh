using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementTextBlock : ElementBaseRectangle, IElementTextBlock, IPrimitive
	{
		public ElementTextBlock()
		{
			Stretch = true;
			TextAlignment = 0;
		}

		[DataMember]
		public string Text { get; set; }
		[DataMember]
		public Color ForegroundColor { get; set; }
		[DataMember]
		public double FontSize { get; set; }
		[DataMember]
		public string FontFamilyName { get; set; }
		[DataMember]
		public bool Stretch { get; set; }
		[DataMember]
		public int TextAlignment { get; set; }

		public override ElementBase Clone()
		{
			ElementTextBlock elementBase = new ElementTextBlock()
			{
				Text = Text,
				ForegroundColor = ForegroundColor,
				FontSize = FontSize,
				FontFamilyName = FontFamilyName,
				Stretch = Stretch,
				TextAlignment = TextAlignment,
			};
			Copy(elementBase);
			return elementBase;
		}

		#region IPrimitive Members

		public Primitive Primitive
		{
			get { return Infrustructure.Plans.Elements.Primitive.TextBlock; }
		}

		#endregion

		protected override void SetDefault()
		{
			Text = "Надпись";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			base.SetDefault();
			Height = 22;
			Width = 52;
		}
	}
}