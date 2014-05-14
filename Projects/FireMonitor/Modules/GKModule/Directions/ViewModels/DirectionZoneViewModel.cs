using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class DirectionZoneViewModel : ZoneViewModel
	{
		public DirectionZoneViewModel(XZone zone)
			: base(zone)
		{
		
		}
		public XStateBit StateType { get; set; }
	}
}