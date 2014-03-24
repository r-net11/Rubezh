using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace SKDModule.ViewModels
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