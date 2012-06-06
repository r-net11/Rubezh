using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Windows
{
	public class LoadingService
	{
		private static ProgressViewModel _progress = null;

		public static void Show(string title, int stepCount = 1)
		{
			Close();
			_progress = new ProgressViewModel() { Title = title, StepCount = stepCount };
			DialogService.ShowWindow(_progress);
		}
		public static void Close()
		{
			if (_progress != null)
				_progress.ForceClose();
			_progress = null;
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
