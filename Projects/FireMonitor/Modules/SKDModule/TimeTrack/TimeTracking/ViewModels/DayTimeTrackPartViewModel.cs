using System;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DayTimeTrackPartViewModel : BaseViewModel, IDataErrorInfo
	{
		public Guid UID { get; private set; }
		public string EnterTime { get { return GetTimeString(EnterTimeSpan); } }
		public string ExitTime { get { return GetTimeString(ExitTimeSpan); } }
		public int No { get; private set; }

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

		public DayTimeTrackPartViewModel(TimeTrackPartDetailsViewModel timeTrackPartDetailsViewModel)
		{
			UID = timeTrackPartDetailsViewModel.UID;
			Update(timeTrackPartDetailsViewModel.EnterTime, timeTrackPartDetailsViewModel.ExitTime, timeTrackPartDetailsViewModel.SelectedZone.Name, timeTrackPartDetailsViewModel.SelectedZone.No);
		}

		public void Update(TimeSpan enterTime, TimeSpan exitTime, string zoneName, int no)
		{
			ZoneName = zoneName ?? "<Нет в конфигурации>";
			No = no;
			EnterTimeSpan = enterTime;
			ExitTimeSpan = exitTime;
		}

		public DayTimeTrackPartViewModel(TimeTrackPart timeTrackPart)
		{
			string zoneName = null;
			var num = default(int);

			var strazhZone = SKDManager.Zones.FirstOrDefault(x => x.UID == timeTrackPart.ZoneUID);
			if (strazhZone != null)
			{
				zoneName = strazhZone.Name;
				num = strazhZone.No;
			}

			UID = timeTrackPart.PassJournalUID;
			Update(timeTrackPart.StartTime, timeTrackPart.EndTime, zoneName, num);
		}

		string GetTimeString(TimeSpan timeSpan)
		{
			return timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
		}

		public bool IsValid
		{
			get { return string.IsNullOrEmpty(Error); }
		}

		public string Error
		{
			get
			{
				return this[string.Empty];
			}
		}

		public string this[string propertyName]
		{
			get
			{
				var result = string.Empty;
				propertyName = propertyName ?? string.Empty;
				if (propertyName == string.Empty || propertyName == "EnterTime")
				{
					if (EnterTimeSpan > ExitTimeSpan)
						result = "Время входа не может быть больше времени выхода";
				}
				if (propertyName == string.Empty || propertyName == "ExitTime")
				{
					if (EnterTimeSpan == ExitTimeSpan)
						result = "Невозможно добавить нулевое пребывание в зоне";
				}
				return result;
			}
		}
	}
}
