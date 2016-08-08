using SKDModule.PassCardDesigner.Model;
using System.Drawing;
using System.IO;

namespace SKDModule.Reports
{
	public static class ReportExtensions
	{
		public static byte[] ToBytes(this PassCardTemplateReport report)
		{
			if (report == null) return null;

			using (var ms = new MemoryStream())
			{
				report.SaveLayout(ms);
				return ms.ToArray();
			}
		}

		public static PassCardTemplateReport ToXtraReport(this byte[] bytes, byte[] imageContent)
		{
			if (bytes == null) return null;

			var report = new PassCardTemplateReport();
			Image image = null;

			if (imageContent != null)
				using (var imageStream = new MemoryStream(imageContent))
					image = Image.FromStream(imageStream);

			using (var ms = new MemoryStream(bytes))
			{
				report.LoadLayout(ms);
				report.Watermark.Image = image;
			}

			return report;
		}
	}
}
