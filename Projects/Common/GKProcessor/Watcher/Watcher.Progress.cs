using Infrastructure.Common.Windows;

namespace GKProcessor
{
	public partial class Watcher
	{
		public static bool MustShowProgress = true;

		void StartProgress(string name, int count, bool canCancel = true)
		{
			if (MustShowProgress)
			{
				ApplicationService.Invoke(() =>
				{
					LoadingService.Show(name, count, canCancel);
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