using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSAgentServer.ViewModels
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			if (MainViewModel.Current != null)
			{
				MainViewModel.Current.AddLog(message, isError);
			}
		}
	}
}