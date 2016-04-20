using System;
using System.Linq;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.ViewModels
{
	public class DayTimeTrackPartViewModel : BaseViewModel
	{
		public Guid UID { get; private set; }
		public string EnterTime { get { return GetTimeString(EnterTimeSpan); } }
		public string ExitTime { get { return GetTimeString(ExitTimeSpan); } }

		TimeSpan _EnterTimeSpan;
		public TimeSpan EnterTimeSpan
		{
			get { return _EnterTimeSpan; }
			set
			{
				_EnterTimeSpan = value;
				OnPropertyChanged(() => EnterTimeSpan);
				OnPropertyChanged(() => EnterTime);
			}
		}

		TimeSpan _ExitTimeSpan;
		public TimeSpan ExitTimeSpan
		{
			get { return _ExitTimeSpan; }
			set
			{
				_ExitTimeSpan = value;
				OnPropertyChanged(() => ExitTimeSpan);
				OnPropertyChanged(() => ExitTime);
			}
		}

		string _ZoneName;
		public string ZoneName
		{
			get { return _ZoneName; }
			set
			{
				_ZoneName = value;
				OnPropertyChanged(() => ZoneName);
			}
		}

		public DayTimeTrackPartViewModel(Guid uid, TimeSpan enterTime, TimeSpan exitTime, string zoneName)
		{
			UID = uid;
			Update(enterTime, exitTime, zoneName);
		}

		public void Update(TimeSpan enterTime, TimeSpan exitTime, string zoneName)
		{
			ZoneName = zoneName != null ? zoneName : "<Нет в конфигурации>";
			EnterTimeSpan = enterTime;
			ExitTimeSpan = exitTime;
		}

		public DayTimeTrackPartViewModel(TimeTrackPart timeTrackPart)
		{
			string zoneName = null;
			var gkZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
			if (gkZone != null)
				zoneName = gkZone.PresentationName;

			UID = timeTrackPart.PassJournalUID;
			Update(timeTrackPart.StartTime, timeTrackPart.EndTime, zoneName);
		}

		string GetTimeString(TimeSpan timeSpan)
		{
			string result;
			if (timeSpan.Days > 0)
				result = "24:00:00";
			else
				result = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
			return result;
		}
	}
}
