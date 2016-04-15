using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Controls.PDF
{
	public static class PDFStyle
	{
		public static BaseFont BaseFont { get; private set; }
		public static Font NormalFont { get; private set; }
		public static Font BoldFont { get; private set; }

		public static Font TextFont { get; private set; }
		public static Font BoldTextFont { get; private set; }
		public static Font HeaderFont { get; private set; }
		public static BaseColor HeaderBackground { get; private set; }

		static PDFStyle()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.TTF");
			BaseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
			NormalFont = new Font(BaseFont, Font.DEFAULTSIZE, Font.NORMAL);
			BoldFont = new Font(BaseFont, Font.DEFAULTSIZE, Font.BOLD);

			TextFont = new Font(BaseFont, 12, Font.NORMAL);
			BoldTextFont = new Font(BaseFont, 12, Font.BOLD);
			HeaderFont = new Font(BaseFont, 16, Font.BOLD);
			HeaderBackground = new BaseColor(unchecked((int)0xFFD3D3D3));
		}
	}
}
