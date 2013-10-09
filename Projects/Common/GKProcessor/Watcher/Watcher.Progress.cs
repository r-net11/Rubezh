using Infrastructure.Common.Windows;

namespace GKProcessor
{
	public partial class Watcher
	{
		public static bool MustShowProgress = true;

		void StartProgress(string name, int count)
		{
			if (MustShowProgress)
			{
				ApplicationService.Invoke(() =>
				{
					LoadingService.ShowWithCancel(name, count);
				});
			}
		}

		void DoProgress(string name)
		{
			if (MustShowProgress)
			{
				ApplicationService.Invoke(() =>
				{
					LoadingService.DoStep(name);
				});
			}
		}

		void StopProgress()
		{
			if (MustShowProgress)
			{
				ApplicationService.Invoke(() =>
				{
					LoadingService.Close();
				});
			}
		}
	}
}