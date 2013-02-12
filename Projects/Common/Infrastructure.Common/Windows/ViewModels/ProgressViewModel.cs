
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true)
		{
			Sizable = false;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
			IsIndeterminate = false;
		}

		public string Text { get; set; }

		private int _currentStep;
		public int CurrentStep
		{
			get { return _currentStep; }
			set
			{
				_currentStep = value;
				OnPropertyChanged("CurrentStep");
			}
		}
		private int _stepCount;
		public int StepCount
		{
			get { return _stepCount; }
			set
			{
				_stepCount = value;
				OnPropertyChanged("StepCount");ApplicationService.DoEvents();
			}
		}
		private bool _isIndeterminate;
		public bool IsIndeterminate
		{
			get { return _isIndeterminate; }
			set
			{
				_isIndeterminate = value;
				OnPropertyChanged("IsIndeterminate");
			}
		}

		public void DoStep(string text)
		{
			CurrentStep++;
			Title = text;
			ApplicationService.DoEvents();
		}

		public bool RestrictClose { get; private set; }

		public override bool OnClosing(bool isCanceled)
		{
			return RestrictClose;
		}
		public void ForceClose()
		{
			RestrictClose = false;
			Close();
		}
	}
}
