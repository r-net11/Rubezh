
namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true, bool canCancel = false)
		{
			Sizable = false;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
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

		public bool IsCanceled { get; set; }

		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			IsCanceled = true;
		}
	}
}