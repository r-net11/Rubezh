
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true)
		{
			Sizable = false;
			TopMost = true;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
		}

		public string Text {get;set;}

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
				OnPropertyChanged("StepCount");
				ApplicationService.DoEvents();
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
