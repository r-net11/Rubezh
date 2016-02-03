using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmGroupViewModel
	{
		public GKAlarmType AlarmType { get; set; }
		public string AlarmTypeName { get; set; }
		public string GlowColor { get; set; }
		public int Count { get; set; }
		public bool HasAlarms { get; set; }

		public AlarmGroupViewModel()
		{
			
		}

		public AlarmGroupViewModel(GKAlarmType alarmType)
		{
			this.AlarmType = alarmType;
			AlarmTypeName = AlarmType.ToString();
            GlowColor = (string)(new AlarmTypeToColorConverter()).Convert(this.AlarmType, null, null, null);
		}

		public void Update(List<AlarmViewModel> Alarms)
		{
			Count = Alarms.Count;
			HasAlarms = (Count > 0);
		}
	}
}