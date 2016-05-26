namespace StrazhService.ViewModels
{
	public static class UILogger
	{
		public static void Log(string message)
		{
			if (MainViewModel.Current != null)
			{
				MainViewModel.Current.AddLog(message);
			}
		}
	}
}