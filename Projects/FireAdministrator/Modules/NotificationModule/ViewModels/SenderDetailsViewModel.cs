using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class SenderDetailsViewModel : SaveCancelDialogViewModel
	{
		public SenderParamsViewModel SenderParamsViewModel { get; private set; }

		public SenderDetailsViewModel()
		{
			SenderParamsViewModel = new SenderParamsViewModel();
			SetDefaultSenderParamsCommand = new RelayCommand(OnSetDefaultSenderParams);
		}

		public SenderDetailsViewModel(SenderParams senderParams)
		{
			SenderParamsViewModel = new SenderParamsViewModel(senderParams);
			SetDefaultSenderParamsCommand = new RelayCommand(OnSetDefaultSenderParams);
		}

		public RelayCommand SetDefaultSenderParamsCommand { get; set; }

		private void OnSetDefaultSenderParams()
		{
			SenderParamsViewModel.SenderParams = SenderParams.SetDefaultParams();
			SenderParamsViewModel.Update();
		}
	}
}