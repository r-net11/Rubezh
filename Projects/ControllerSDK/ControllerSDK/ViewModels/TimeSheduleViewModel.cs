using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using ControllerSDK.API;

namespace ControllerSDK.ViewModels
{
	public class TimeSheduleViewModel : BaseViewModel
	{
		public TimeSheduleViewModel(TimeShedule timeShedule)
		{
			StartTime = timeShedule.BeginHours.ToString() + ":" + timeShedule.BeginMinutes.ToString() + ":" + timeShedule.BeginSeconds.ToString();
			EndTime = timeShedule.EndHours.ToString() + ":" + timeShedule.EndMinutes.ToString() + ":" + timeShedule.EndSeconds.ToString();
		}

		public int DoorNo { get; set; }
		public int DayNo { get; set; }
		public string StartTime { get; private set; }
		public string EndTime { get; private set; }
	}
}