using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infrastructure.Common.Windows;

namespace ServerFS2.ViewModels
{
	public static class LogService
	{
		public static void AddUSBHidLog(FS2LogItem value)
		{
			if (LogsViewModel.Current != null)
			{
				LogsViewModel.Current.AddLog(value);
			}
		}
	}
}