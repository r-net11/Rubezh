using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class CableTypeViewModel : BaseViewModel
	{
		public CableTypeViewModel(CableType cableType)
		{
			CableType = cableType;
		}

		public CableType CableType { get; private set; }
	}
}