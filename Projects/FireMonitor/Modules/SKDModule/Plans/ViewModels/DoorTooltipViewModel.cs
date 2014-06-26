using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DoorTooltipViewModel : BaseViewModel
	{
		public Door Door { get; private set; }

		public DoorTooltipViewModel(Door door)
		{
			Door = door;
		}

		public void OnStateChanged()
		{
			OnPropertyChanged(() => Door);
		}
	}
}