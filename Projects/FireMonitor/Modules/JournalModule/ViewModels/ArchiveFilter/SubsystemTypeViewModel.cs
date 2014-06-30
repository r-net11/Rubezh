using FiresecAPI.SKD;
using Infrastructure.Common.CheckBoxList;

namespace JournalModule.ViewModels
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