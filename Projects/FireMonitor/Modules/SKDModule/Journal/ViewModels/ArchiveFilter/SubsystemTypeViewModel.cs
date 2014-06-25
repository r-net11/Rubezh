using FiresecAPI.SKD;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class SubsystemTypeViewModel : CheckBoxItemViewModel
	{
		public SubsystemTypeViewModel(SubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
		}

		public SubsystemType SubsystemType { get; private set; }
	}
}