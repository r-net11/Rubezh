using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public List<string> Operations { get; private set; }
	}
}