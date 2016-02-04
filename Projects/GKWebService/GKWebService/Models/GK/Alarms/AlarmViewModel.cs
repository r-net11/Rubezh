using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmViewModel
	{
		public string ObjectName { get; set; }

		public string ImageSource { get; set; }

		public AlarmViewModel(Alarm alarm)
		{
			ObjectName = alarm.GkBaseEntity.PresentationName;
			ImageSource = alarm.GkBaseEntity.ImageSource;
		}
	}
}