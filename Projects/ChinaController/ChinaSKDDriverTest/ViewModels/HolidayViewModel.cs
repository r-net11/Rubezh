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

			Doors = holiday.DoorsCount.ToString() + "(";
			foreach (var door in holiday.Doors)
			{
				Doors += door.ToString() + ",";
			}
			Doors += ")";
		}

		public string StartTime { get; private set; }
		public string EndTime { get; private set; }
		public string Doors { get; private set; }
	}
}