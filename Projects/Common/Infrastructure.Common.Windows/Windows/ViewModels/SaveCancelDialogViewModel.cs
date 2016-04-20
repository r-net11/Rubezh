
namespace Infrastructure.Common.Windows.Windows.ViewModels
{
	public class SaveCancelDialogViewModel : DialogViewModel
	{
		public RelayCommand SaveCommand { get; protected set; }
		public RelayCommand CancelCommand { get; protected set; }

		public SaveCancelDialogViewModel()
		{
			AllowSave = true;
			SaveCaption = "Ok";
			CancelCaption = "Отмена";
			SaveCommand = new RelayCommand(OnSave, CanSave);
			CancelCommand = new RelayCommand(OnCancel);
			CommandPanel = null;
		}

		private bool _allowSave;
		public bool AllowSave
		{
			get { return _allowSave; }
			set
			{
				_allowSave = value;
				OnPropertyChanged(() => AllowSave);
			}
		}
		private string _saveCaption;
		public string SaveCaption
		{
			get { return _saveCaption; }
			set
			{
				_saveCaption = value;
				OnPropertyChanged("SaveCaption");
			}
		}
		private string _cancelCaption;
		public string CancelCaption
		{
			get { return _cancelCaption; }
			set
			{
				_cancelCaption = value;
				OnPropertyChanged("CancelCaption");
			}
		}

		private BaseViewModel _commandPanel;
		public BaseViewModel CommandPanel
		{
			get { return _commandPanel; }
			set
			{
				_commandPanel = value;
				OnPropertyChanged(() => CommandPanel);
			}
		}

		protected virtual bool CanSave()
		{
			return true;
		}
		protected virtual bool Save()
		{
			return true;
		}
		protected virtual bool Cancel()
		{
			return false;
		}

		private void OnSave()
		{
			bool result = Save();
			if (result)
				Close(true);
		}
		private void OnCancel()
		{
			bool result = Cancel();
			Close(result);
		}
	}
}
