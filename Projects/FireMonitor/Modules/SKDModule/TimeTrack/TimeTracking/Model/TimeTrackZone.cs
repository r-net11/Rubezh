using System;
using FiresecAPI.SKD;

namespace SKDModule.Model
{
	public class TimeTrackZone
	{
		public SKDZone SKDZone { get; private set; }
		public Guid UID { get; private set; }
		public int No { get; set; }
		public string Name { get; set; }
		public string Description { get; private set; }
		public bool IsURV { get; set; }

		public TimeTrackZone(SKDZone zone)
		{
			SKDZone = zone;
			UID = zone.UID;
			No = zone.No;
			Name = zone.Name;
			Description = zone.Description;
		}

		public TimeTrackZone()
		{
		}
	}
}
