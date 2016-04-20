using System;
using Common;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Windows
{
	public class LoadingService
	{
		private static ProgressViewModel _progressViewModel = null;

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

		public static void DoStep(string text)
		{
			if (_progressViewModel != null)
			{
				_progressViewModel.DoStep(text);
			}
		}

		public static void DoStep(string text, string title, int stepCount, int currentStep, bool canCancel)
		{
			try
			{
				if (_progressViewModel == null)
				{
					Show(title, text, stepCount, canCancel);
				}
				if (_progressViewModel != null)
				{
					_progressViewModel.Text = text;
					_progressViewModel.Title = title;
					_progressViewModel.StepCount = stepCount;
					_progressViewModel.CurrentStep = currentStep;
					_progressViewModel.CanCancel = canCancel;
					_progressViewModel.DoStep(text);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "LoadingService.DoStep " + text + " # " + title + " # " + stepCount);
			}
		}
		public static void AddCount(int count)
		{
			if (_progressViewModel != null)
				_progressViewModel.StepCount += count;
		}
	}
}