using Infrastructure.Common.CheckBoxList;
using XFiresecAPI;

namespace SKDModule.ViewModels
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