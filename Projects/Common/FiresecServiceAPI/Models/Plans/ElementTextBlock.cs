﻿using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementTextBlock : ElementBaseRectangle, IElementTextBlock, IElementZIndex
	{
		public ElementTextBlock()
		{
			Text = "Надпись";
			ForegroundColor = Colors.Black;
			FontSize = 10;
			FontFamilyName = "Arial";
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
		public int ZIndex { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementTextBlock()
			{
				Text = Text,
				BackgroundColor = BackgroundColor,
				ForegroundColor = ForegroundColor,
				BorderColor = BorderColor,
				BorderThickness = BorderThickness,
				FontSize = FontSize,
				FontFamilyName = FontFamilyName
			};
			Copy(elementBase);
			return elementBase;
		}
	}
}