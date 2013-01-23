using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public class LoadingService
	{
		private static ProgressViewModel _progress = null;

		public static void SaveShowProgress(string name, int count)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.ShowProgress("", name, count);
			});
		}
		public static void SaveDoStep(string name)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.DoStep(name);
			});
		}
		public static void SaveClose()
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.Close();
			});
		}

		public static void ShowProgress(string title, string text, int stepCount = 1)
		{
			Show(new ProgressViewModel() { Title = title, StepCount = stepCount, Text = text });
		}
		public static void ShowProgressIndeterminate(string title, string text)
		{
			Show(new ProgressViewModel() { Title = title, IsIndeterminate = true, Text = text });
		}
		public static void Show(string title, int stepCount = 1)
		{
			Show(new LoadingViewModel() { Title = title, StepCount = stepCount });
		}
		private static void Show(ProgressViewModel progressViewModel)
		{
			Close();
			_progress = progressViewModel;
			DialogService.ShowWindow(_progress);
			if (ApplicationService.ApplicationWindow != null)
				ApplicationService.Invoke(() => ApplicationService.ApplicationWindow.IsHitTestVisible = false);
		}
		public static void Close()
		{
			if (_progress != null)
				_progress.ForceClose();
			_progress = null;
			if (ApplicationService.ApplicationWindow != null)
				ApplicationService.Invoke(() => ApplicationService.ApplicationWindow.IsHitTestVisible = true);
		}

		public static void DoStep(string title)
		{
			if (_progress != null)
				_progress.DoStep(title);
		}
		public static void AddCount(int count)
		{
			if (_progress != null)
				_progress.StepCount += count;
		}
	}
}