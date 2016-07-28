using System;
using System.Diagnostics;
using System.IO;
using Common;

namespace Infrastructure.Common
{
	public static class ManualPdfHelper
	{
		public static void Show(string manualName)
		{
			if (string.IsNullOrEmpty(manualName)) return;

			var fileName = AppDataFolderHelper.GetFile(manualName);

			if (!File.Exists(fileName)) return;

			try
			{
				Process.Start(fileName);
			}
			catch (Exception e)
			{
				Logger.Error(e);
				throw;
			}
		}
	}
}