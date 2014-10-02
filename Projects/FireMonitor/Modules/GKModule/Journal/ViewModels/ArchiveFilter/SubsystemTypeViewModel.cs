using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class SubsystemTypeViewModel : CheckBoxItemViewModel
	{
		public SubsystemTypeViewModel(GKSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
		}

		public GKSubsystemType SubsystemType { get; private set; }
	}
}