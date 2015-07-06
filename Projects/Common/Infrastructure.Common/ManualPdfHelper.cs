using System.Diagnostics;
using System.IO;

namespace Infrastructure.Common
{
	public static class ManualPdfHelper
	{
		public static void Show(string manualName)
		{
			if (string.IsNullOrEmpty(manualName)) return;

			var fileName = AppDataFolderHelper.GetFile(manualName);

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