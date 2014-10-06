using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveGuardZoneViewModel : CheckBoxItemViewModel
	{
		public ArchiveGuardZoneViewModel(GKGuardZone guardZone)
		{
			GuardZone = guardZone;
			Name = guardZone.PresentationName;
		}

		public GKGuardZone GuardZone { get; private set; }
		public string Name { get; private set; }
	}
}