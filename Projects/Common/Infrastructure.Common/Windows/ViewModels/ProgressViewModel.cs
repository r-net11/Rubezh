
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true, bool canCancel = false)
		{
			Sizable = false;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
			IsIndeterminate = false;
			CanCancel = canCancel;
			CancelCommand = new RelayCommand(OnCancel);
		}

		string _text;
		public string Text 
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
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
				OnPropertyChanged("StepCount"); 
				ApplicationService.DoEvents();
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
			Text = text;
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

		public bool CanCancel { get; set; }
		public bool IsCanceled { get; private set; }

		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			IsCanceled = true;
		}
	}
}