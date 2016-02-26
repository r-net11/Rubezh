using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Controls.Converters;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmGroupViewModel
	{
		public GKAlarmType AlarmType { get; set; }
		public string AlarmTypeName { get; set; }
		public string GlowColor { get; set; }
		public string AlarmImageSource { get; set; }
		public string AlarmName { get; set; }
		public int Count { get; set; }
		public bool HasAlarms { get; set; }
		public List<ShortAlarmObject> Alarms { get; set; }
		
		public AlarmGroupViewModel()
		{
			
		}

		public AlarmGroupViewModel(GKAlarmType alarmType)
		{
			this.AlarmType = alarmType;
			AlarmTypeName = AlarmType.ToString();
            GlowColor = new AlarmTypeToColorConverter().Convert(this.AlarmType);
			AlarmImageSource = ((string)new AlarmTypeToBIconConverter().Convert(alarmType, null, null, null)).Substring(36).Replace(".png", "");
			AlarmName = alarmType.ToDescription();
		}

		public void Update(List<AlarmViewModel> Alarms)
		{
			Count = Alarms.Count;
			HasAlarms = (Count > 0);
			this.Alarms = Alarms.Select(a => new ShortAlarmObject(a)).ToList();
		}
	}

	public class ShortAlarmObject
	{
		public string ObjectName { get; set; }

		public string ObjectImageSource { get; set; }

		public ShortAlarmObject()
		{

		}
		public ShortAlarmObject(AlarmViewModel model)
		{
			ObjectName = model.GkEntity.Name;
			ObjectImageSource = model.GkEntity.ImageSource;
		}
	}
}