using System.Diagnostics;
using System.IO;

namespace Infrastructure.Common.Windows
{
	public static class ManualPdfHelper
	{
		public static void Show()
		{
			var fileName = Infrastructure.Common.Windows.AppDataFolderHelper.GetFile("Manual.pdf");
			if (File.Exists(fileName))
			{
				try
				{
					Process.Start(fileName);
				}
				catch
				{

				}
			}
		}
	}
}