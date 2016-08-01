using System.ComponentModel;
using System.Drawing;
using System.IO;
using ReportSystem.UI.Reports;

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

			Image image = null;

			if (imageContent != null)
				using (var imageStream = new MemoryStream(imageContent))
					image = Image.FromStream(imageStream);

			using (var ms = new MemoryStream(bytes))
			{
				DXDisplayNameAttribute.UseResourceManager = true;
				var report = new PassCardTemplateReport(image);
				report.LoadLayout(ms);
				report.Watermark.Image = image;
				return report;
			}
		}
	}
}
