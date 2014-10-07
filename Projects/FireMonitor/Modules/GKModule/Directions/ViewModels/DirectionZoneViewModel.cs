using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class DirectionZoneViewModel : ZoneViewModel
	{
		public DirectionZoneViewModel(GKZone zone)
			: base(zone)
		{
		
		}
		public GKStateBit StateType { get; set; }
	}
}