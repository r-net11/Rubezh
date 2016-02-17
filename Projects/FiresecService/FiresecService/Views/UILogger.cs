using Infrastructure.Common.BalloonTrayTip;
namespace FiresecService.Models
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			if (MainViewModel.Current != null)
			{
				MainViewModel.Current.AddLog(message, isError);
			}
			if(isError)
			{
				BalloonHelper.ShowFromServer(message);
			}
		}
	}
}