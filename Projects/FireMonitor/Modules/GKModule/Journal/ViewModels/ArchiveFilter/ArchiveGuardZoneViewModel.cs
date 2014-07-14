using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveGuardZoneViewModel : CheckBoxItemViewModel
	{
		public ArchiveGuardZoneViewModel(XGuardZone guardZone)
		{
			GuardZone = guardZone;
			Name = guardZone.PresentationName;
		}

		public XGuardZone GuardZone { get; private set; }
		public string Name { get; private set; }
	}
}