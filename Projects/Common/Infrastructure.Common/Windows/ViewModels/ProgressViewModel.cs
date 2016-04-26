namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel()
		{
			CancelCommand = new RelayCommand(OnCancel);
			Sizable = false;
			RestrictClose = true;
			HideInTaskbar = true;
			CanCancel = false;
		}

		public bool RestrictClose { get; private set; }

		public bool IsCanceled { get; set; }

		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		private int _currentStep;

		public int CurrentStep
		{
			get { return _currentStep; }
			set
			{
				_currentStep = value;
				OnPropertyChanged(() => CurrentStep);
			}
		}

		private int _stepCount;

		public int StepCount
		{
			get { return _stepCount; }
			set
			{
				_stepCount = value;
				OnPropertyChanged(() => StepCount);
				if (ApplicationService.IsApplicationThread())
					ApplicationService.DoEvents();
			}
		}

		private bool _canCancel;

		public bool CanCancel
		{
			get { return _canCancel; }
			set
			{
				if (value)
					CancelText = Resources.Language.ProgressViewModel.CancelText_True;
				else
                    CancelText = Resources.Language.ProgressViewModel.CancelText_False;

				_canCancel = value;
				OnPropertyChanged(() => CanCancel);
			}
		}

		private string _cancelText;

		public string CancelText
		{
			get { return _cancelText; }
			set
			{
				_cancelText = value;
				OnPropertyChanged(() => CancelText);
			}
		}

		public void DoStep(string text)
		{
			CurrentStep++;
			Text = text;
			if (ApplicationService.IsApplicationThread())
				ApplicationService.DoEvents();
		}

		public void DoStep()
		{
			if (ApplicationService.IsApplicationThread())
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

		protected virtual void OnCancel()
		{
			IsCanceled = true;
			Close();
		}
	}
}