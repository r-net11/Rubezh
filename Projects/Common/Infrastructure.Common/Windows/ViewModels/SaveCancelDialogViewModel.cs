using System.Windows.Media.Converters;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class SaveCancelDialogViewModel : DialogViewModel
	{
		private bool _isCancelled;

		public bool IsCancelled
		{
			get { return _isCancelled; }
			set
			{
				if (_isCancelled == value) return;
				_isCancelled = value;
				OnPropertyChanged(() => IsCancelled);
			}
		}

		public RelayCommand SaveCommand { get; protected set; }

		public RelayCommand CancelCommand { get; protected set; }

		public SaveCancelDialogViewModel()
		{
			AllowSave = true;
			SaveCaption = CommonResources.OK;
			CancelCaption = CommonResources.Cancel;
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

		public virtual string SaveCaption
		{
			get { return _saveCaption; }
			set
			{
				_saveCaption = value;
				OnPropertyChanged(() => SaveCaption);
			}
		}

		private string _cancelCaption;

		public virtual string CancelCaption
		{
			get { return _cancelCaption; }
			set
			{
				_cancelCaption = value;
				OnPropertyChanged(() => CancelCaption);
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
			IsCancelled = true;
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
			IsCancelled = ! result;
			if (IsCancelled && !AllowClose) //TODO: Bad realisation. Need to fix SKDDEV-839.
				AllowClose = true;

			CloseCanBeCancelled(result, IsCancelled);
		}
	}
}