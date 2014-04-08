using Infrastructure.Common.CheckBoxList;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class SubsystemTypeViewModel : CheckBoxItemViewModel
	{
		public SubsystemTypeViewModel(SKDSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
		}

		public SKDSubsystemType SubsystemType { get; private set; }
	}
}