using System.Diagnostics;
using System.IO;

namespace Infrastructure.Common
{
	public static class ManualPdfHelper
	{
		public static void Show(string manualName = null)
		{
			var fileName = AppDataFolderHelper.GetFile(manualName ?? "Manual.pdf");
			
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