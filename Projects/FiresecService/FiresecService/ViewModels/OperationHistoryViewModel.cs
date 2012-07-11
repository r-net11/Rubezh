using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class OperationHistoryViewModel : SaveCancelDialogViewModel
	{
		public OperationHistoryViewModel(ClientViewModel clientViewModel)
		{
			Title = "История операций";
			Operations = clientViewModel.Operations;
			AllowSave = false;
			CancelCaption = "Закрыть";
		}

		public List<OperationViewModel> Operations { get; private set; }
	}
}