using StrazhDeviceSDK.API;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class TimeSheduleViewModel : BaseViewModel
	{
		public TimeSheduleViewModel(TimeShedule timeShedule)
		{
			Interval1 = TimeSheduleIntervalToString(timeShedule.TimeSheduleIntervals[0]);
			Interval2 = TimeSheduleIntervalToString(timeShedule.TimeSheduleIntervals[1]);
			Interval3 = TimeSheduleIntervalToString(timeShedule.TimeSheduleIntervals[2]);
			Interval4 = TimeSheduleIntervalToString(timeShedule.TimeSheduleIntervals[3]);
		}

		public string Interval1 { get; set; }
		public string Interval2 { get; set; }
		public string Interval3 { get; private set; }
		public string Interval4 { get; private set; }

		public string TimeSheduleIntervalToString(TimeSheduleInterval timeSheduleInterval)
		{
			var startTime = timeSheduleInterval.BeginHours.ToString() + ":" + timeSheduleInterval.BeginMinutes.ToString() + ":" + timeSheduleInterval.BeginSeconds.ToString();
			var endTime = timeSheduleInterval.EndHours.ToString() + ":" + timeSheduleInterval.EndMinutes.ToString() + ":" + timeSheduleInterval.EndSeconds.ToString();
			return startTime + " - " + endTime;
		}
	}
}