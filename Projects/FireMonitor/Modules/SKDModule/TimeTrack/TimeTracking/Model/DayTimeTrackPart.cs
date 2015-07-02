using FiresecAPI.SKD;
using ReactiveUI;
using SKDModule.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

namespace SKDModule.Model
{
	public class DayTimeTrackPart : ReactiveObject, IDataErrorInfo
	{
		#region Properties

		public Guid UID { get; private set; }
		public int No { get; private set; }

		TimeSpan _enterTimeSpan;
		public TimeSpan EnterTimeSpan
		{
			get { return _enterTimeSpan; }
			set { this.RaiseAndSetIfChanged(ref _enterTimeSpan, value); }
		}

		TimeSpan _exitTimeSpan;
		public TimeSpan ExitTimeSpan
		{
			get { return _exitTimeSpan; }
			set { this.RaiseAndSetIfChanged(ref _exitTimeSpan, value); }
		}

		string _zoneName;
		public string ZoneName
		{
			get { return _zoneName; }
			set { this.RaiseAndSetIfChanged(ref _zoneName, value); }
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

		#endregion

		#region Constructors

		public DayTimeTrackPart(TimeTrackPartDetailsViewModel timeTrackPartDetailsViewModel)
		{
			UID = timeTrackPartDetailsViewModel.UID;
			Update(timeTrackPartDetailsViewModel.EnterTime, timeTrackPartDetailsViewModel.ExitTime, timeTrackPartDetailsViewModel.SelectedZone.Name, timeTrackPartDetailsViewModel.SelectedZone.No);
		}

		public DayTimeTrackPart(TimeTrackPart timeTrackPart)
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

		public DayTimeTrackPart()
		{
		}

		#endregion

		#region Methods

		public void Update(TimeSpan enterTime, TimeSpan exitTime, string zoneName, int no)
		{
			ZoneName = zoneName ?? "<Нет в конфигурации>";
			No = no;
			EnterTimeSpan = enterTime;
			ExitTimeSpan = exitTime;
		}

		#endregion

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
