using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class ArchiveZoneViewModel : CheckBoxItemViewModel
	{
		public ArchiveZoneViewModel(XZone zone)
		{
			Zone = zone;
			Name = zone.PresentationName;
		}

		public XZone Zone { get; private set; }
		public string Name { get; private set; }
	}
}