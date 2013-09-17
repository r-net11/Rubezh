using Infrastructure.Common.Windows;

namespace GKProcessor
{
	public partial class Watcher
	{
		void StartProgress(string name, int count)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.ShowProgress("", name, count);
			});
		}

		void DoProgress(string name)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.DoStep(name);
			});
		}

		void StopProgress()
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.Close();
			});
		}
	}
}