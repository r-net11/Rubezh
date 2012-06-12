using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class OperationHistoryViewModel : DialogViewModel
	{
		public OperationHistoryViewModel(ClientViewModel clientViewModel)
		{
			Title = "История операций";
			Operations = clientViewModel.Operations;
		}

		public List<OperationViewModel> Operations { get; private set; }
	}
}