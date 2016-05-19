
namespace RubezhService.Presenters
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			if (MainPresenter.Current != null)
			{
				MainPresenter.Current.AddLog(message, isError);
			}
		}
	}
}
