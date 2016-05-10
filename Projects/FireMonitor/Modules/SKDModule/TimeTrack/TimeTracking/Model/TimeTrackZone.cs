using System;
using StrazhAPI.SKD;

namespace SKDModule.Model
{
	public class TimeTrackZone
	{
		public SKDZone SKDZone { get; set; }
		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
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

		public TimeTrackZone(StrazhAPI.SKD.TimeTrackZone timeTrackZone)
		{
			SKDZone = timeTrackZone.SKDZone;
			UID = timeTrackZone.UID;
			No = timeTrackZone.No;
			Name = timeTrackZone.Name;
			Description = timeTrackZone.Description;
			IsURV = timeTrackZone.IsURV;
		}

		public StrazhAPI.SKD.TimeTrackZone ToDTO()
		{
			return new StrazhAPI.SKD.TimeTrackZone
			{
				Description = Description,
				IsURV = IsURV,
				Name = Name,
				No = No,
				SKDZone = SKDZone,
				UID = UID
			};
		}
	}
}
