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

		public static void Show(string title, string text = null, int stepCount = 1, bool canCancel = false)
		{
			if (text == null)
				text = title;
			Show(new ProgressViewModel() { Title = title, StepCount = stepCount, CanCancel = canCancel, Text = text });
		}

		public static void ShowLoading(string title, int stepCount = 1)
		{
			Show(new LoadingViewModel() { Title = title, StepCount = stepCount });
		}

		public static bool IsCanceled
		{
			get { return _progressViewModel != null && _progressViewModel.IsCanceled; }
			set
			{
				if (_progressViewModel != null)
					_progressViewModel.IsCanceled = value;
			}
		}

		static void Show(ProgressViewModel progressViewModel)
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

		public static void DoStep(string title, string text = null, int stepCount = 1, bool canCancel = false)
		{
			if (_progressViewModel == null)
			{
				Show(title, text, stepCount, canCancel);
			}
			if (_progressViewModel != null)
				_progressViewModel.DoStep(text);
		}
		public static void AddCount(int count)
		{
			if (_progressViewModel != null)
				_progressViewModel.StepCount += count;
		}
	}
}