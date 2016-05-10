using FiresecService.Views;

namespace FiresecService.View
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			if (MainView.Current != null)
			{
				MainView.Current.AddLog(message, isError);
			}
			//if (isError)
			//{
			//	Program.ShowBalloonTip(5000, "Ошибка", message, ToolTipIcon.Error);
			//}
		}
	}
}
