using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class CableTypeViewModel : BaseViewModel
	{
		public CableTypeViewModel(Cable cableType)
		{
			CableType = cableType;
		}

		public Cable CableType { get; private set; }
	}
}