using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class HolidayViewModel : BaseViewModel
	{
		public Holiday Holiday { get; private set; }

		public HolidayViewModel(Holiday holiday)
		{
			Holiday = holiday;
			StartTime = holiday.StartDateTime.ToString();
			EndTime = holiday.EndDateTime.ToString();
		}

		public string StartTime { get; private set; }
		public string EndTime { get; private set; }
	}
}