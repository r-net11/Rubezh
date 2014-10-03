using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveMPTViewModel : CheckBoxItemViewModel
	{
		public ArchiveMPTViewModel(GKMPT mpt)
		{
			MPT = mpt;
			Name = mpt.PresentationName;
		}

		public GKMPT MPT { get; private set; }
		public string Name { get; private set; }
	}
}