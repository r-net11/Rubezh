﻿using System.Runtime.Serialization;
using System.Windows.Media;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartProcedureProperties : LayoutPartReferenceProperties
	{
		public LayoutPartProcedureProperties()
		{
			Text = "";
			//ForegroundColor = Colors.Black; //TODO: mde
			//BackgroundColor = Colors.White; //TODO: mde
			//BorderColor = Colors.Black; //TODO: mde
			BorderThickness = 1;
			FontSize = 10;
			TextAlignment = 0;
			FontFamilyName = "Arial";
			FontItalic = false;
			FontBold = false;
		}

		[DataMember]
		public bool UseCustomStyle { get; set; }
		[DataMember]
		public string Text { get; set; }
		[DataMember]
		public Color BorderColor { get; set; }
		[DataMember]
		public double BorderThickness { get; set; }
		[DataMember]
		public Color BackgroundColor { get; set; }
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
	}
}