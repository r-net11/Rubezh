
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel()
		{
			Sizable = false;
			RestrictClose = true;
			HideInTaskbar = true;
			CanCancel = false;
			CancelCommand = new RelayCommand(OnCancel);
		}

		public bool RestrictClose { get; private set; }
		public bool IsCanceled { get; set; }

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
		bool _canCancel;
		public bool CanCancel
		{
			get { return _canCancel; }
			set
			{
				_canCancel = value;
				OnPropertyChanged("CanCancel");
			}
		}

		public void DoStep(string text)
		{
			CurrentStep++;
			Text = text;
			ApplicationService.DoEvents();
		}

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
			IsCanceled = true;
		}
	}
}