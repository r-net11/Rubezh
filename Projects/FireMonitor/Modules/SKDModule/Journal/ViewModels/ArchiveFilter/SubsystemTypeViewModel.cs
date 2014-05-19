using FiresecAPI.SKD;
using Infrastructure.Common.CheckBoxList;

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