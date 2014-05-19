using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class SubsystemTypeViewModel : CheckBoxItemViewModel
	{
		public SubsystemTypeViewModel(XSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
		}

		public XSubsystemType SubsystemType { get; private set; }
	}
}