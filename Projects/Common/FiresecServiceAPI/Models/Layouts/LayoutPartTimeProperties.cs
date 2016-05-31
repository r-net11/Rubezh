using System.Runtime.Serialization;

namespace StrazhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartTimeProperties : ILayoutProperties
	{
		public LayoutPartTimeProperties()
		{
			Format = "dd.MM.yyyy H:mm:ss";
			ForegroundColor = Colors.White;
			BackgroundColor = Colors.Transparent;
			BorderColor = Colors.Black;
			FontSize = 23;
			FontFamilyName = "Arial";
		}

		[DataMember]
		public string Format { get; set; }

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
		public int HorizontalAlignment { get; set; }

		[DataMember]
		public int VerticalAlignment { get; set; }
	}
}