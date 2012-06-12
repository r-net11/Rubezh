using System;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class OperationViewModel : BaseViewModel
	{
		public DateTime StartDateTime { get; set; }
		public TimeSpan Duration { get; set; }
		public string OperationName { get; set; }
		public OperationDirection Direction { get; set; }
	}
}