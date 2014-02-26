using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

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