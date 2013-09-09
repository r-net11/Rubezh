
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true, bool isCancelVisible = false)
		{
			Sizable = false;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
			IsIndeterminate = false;
			IsCancelVisible = isCancelVisible;
			CancelCommand = new RelayCommand(OnCancel);
		}

		public string Text { get; set; }

		bool _isCancelVisible;
		public bool IsCancelVisible
		{
			get { return _isCancelVisible; }
			set
			{
				_isCancelVisible = value;
				OnPropertyChanged("IsCancelVisible");
			}
		}

		int _currentStep;
		public int CurrentStep
		{
			get { return _currentStep; }
			set
			{
				_currentStep = value;
				OnPropertyChanged("CurrentStep");
			}
		}
		int _stepCount;
		public int StepCount
		{
			get { return _stepCount; }
			set
			{
				_stepCount = value;
				OnPropertyChanged("StepCount"); ApplicationService.DoEvents();
			}
		}
		bool _isIndeterminate;
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

		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
		
		}
	}
}