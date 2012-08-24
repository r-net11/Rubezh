using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace FiltersModule.ViewModels
{
	public class StateTypeViewModel : BaseViewModel
	{
		public StateTypeViewModel(StateType stateType)
		{
			StateType = stateType;
		}

		public StateType StateType { get; private set; }
		public bool IsChecked { get; set; }
	}
}