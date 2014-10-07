using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveZoneViewModel : CheckBoxItemViewModel
	{
		public ArchiveZoneViewModel(GKZone zone)
		{
			Zone = zone;
			Name = zone.PresentationName;
		}

		public GKZone Zone { get; private set; }
		public string Name { get; private set; }
	}
}