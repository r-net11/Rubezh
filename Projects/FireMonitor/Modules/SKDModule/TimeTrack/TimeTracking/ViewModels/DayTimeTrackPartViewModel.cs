using System;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

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

		public DayTimeTrackPartViewModel(Guid uid, TimeSpan enterTime, TimeSpan exitTime, SKDZone zone)
		{
			UID = uid;
			Update(enterTime, exitTime, zone);
		}

		public void Update(TimeSpan enterTime, TimeSpan exitTime, SKDZone zone)
		{
			ZoneName = zone != null ? zone.Name : "<Нет в конфигурации>";
			EnterTimeSpan = enterTime;
			ExitTimeSpan = exitTime;
		}

		public DayTimeTrackPartViewModel(TimeTrackPart timeTrackPart)
			: this(timeTrackPart.PassJournalUID, timeTrackPart.StartTime, timeTrackPart.EndTime, SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID)) { }

		string GetTimeString(TimeSpan timeSpan)
		{
			return timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
		}
	}
}
