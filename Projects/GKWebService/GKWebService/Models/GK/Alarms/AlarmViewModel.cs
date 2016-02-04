using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Controls.Converters;
using RubezhAPI;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmViewModel
	{
		public string AlarmImageSource { get; set; }

		public string AlarmName { get; set; }

		public string ObjectName { get; set; }

		public string ObjectImageSource { get; set; }

		public string ObjectStateClass { get; set; }

		public AlarmViewModel(Alarm alarm)
		{
			AlarmImageSource = ((string)new AlarmTypeToBIconConverter().Convert(alarm.AlarmType, null, null, null)).Substring(35);
			AlarmName = alarm.AlarmType.ToDescription();
            ObjectName = alarm.GkBaseEntity.PresentationName;
			ObjectImageSource = alarm.GkBaseEntity.ImageSource;
			ObjectStateClass = alarm.GkBaseEntity.State.StateClass.ToString();
        }
	}
}