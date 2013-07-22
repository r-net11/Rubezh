using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Infrastructure.Common
{
	public static class ManualPdfHelper
	{
		public static void Show()
		{
			var fileName = Infrastructure.Common.AppDataFolderHelper.GetFile("Manual.pdf");
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