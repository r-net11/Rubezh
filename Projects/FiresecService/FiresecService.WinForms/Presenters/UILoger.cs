using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FiresecService.Presenters
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			if (MainPresenter.Current != null)
			{
				MainPresenter.Current.AddLog(message, isError);
			}
			if (isError)
			{
				Program.ShowBalloonTip(5000, "Ошибка", message, ToolTipIcon.Error);
			}
		}
	}
}
