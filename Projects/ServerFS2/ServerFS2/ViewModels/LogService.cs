using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2.ViewModels
{
	public static class LogService
	{
		public static void AddUSBHidLog(string value)
		{
			if (LogsViewModel.Current != null)
			{
				LogsViewModel.Current.AddLog(value);
			}
		}
	}
}