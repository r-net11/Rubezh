using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public class LoadingService
	{
		private static ProgressViewModel _progressViewModel = null;

		public static void SaveShowProgress(string name, int count)
		{
			ApplicationService.Invoke(() =>
			{
				LoadingService.Show("", name, count);
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

		public static void Show(string title, string text, int stepCount = 1)
		{
			Show(new ProgressViewModel() { Title = title, StepCount = stepCount, Text = text });
		}

		public static void Show(string title, int stepCount = 1)
		{
			Show(new ProgressViewModel() { Title = title, StepCount = stepCount, Text = title });
		}
		
		public static void ShowIndeterminate(string title, string text)
		{
			Show(new ProgressViewModel() { Title = title, IsIndeterminate = true, Text = text });
		}

		public static void ShowWithCancel(string title, int stepCount = 1)
		{
			Show(new ProgressViewModel() { Title = title, StepCount = stepCount, CanCancel = true, Text = title });
		}

		public static void ShowLoading(string title, int stepCount = 1)
		{
			Show(new LoadingViewModel() { Title = title, StepCount = stepCount });
		}

		public static bool IsCanceled
		{
			get { return _progressViewModel != null && _progressViewModel.IsCanceled; }
		}

		private static void Show(ProgressViewModel progressViewModel)
		{
			Close();
			_progressViewModel = progressViewModel;
			DialogService.ShowWindow(_progressViewModel);
			if (ApplicationService.ApplicationWindow != null)
				ApplicationService.Invoke(() => ApplicationService.ApplicationWindow.IsHitTestVisible = false);
		}
		public static void Close()
		{
			if (_progressViewModel != null)
				_progressViewModel.ForceClose();
			_progressViewModel = null;
			if (ApplicationService.ApplicationWindow != null)
				ApplicationService.Invoke(() => ApplicationService.ApplicationWindow.IsHitTestVisible = true);
		}

		public static void DoStep(string title)
		{
			if (_progressViewModel != null)
				_progressViewModel.DoStep(title);
		}
		public static void AddCount(int count)
		{
			if (_progressViewModel != null)
				_progressViewModel.StepCount += count;
		}
	}
}