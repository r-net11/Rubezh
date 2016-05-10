using StrazhDeviceSDK.API;
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
			for (int i = 0; i < holiday.DoorsCount; i++)
			{
				Doors += holiday.Doors[i].ToString();
				if (i < holiday.DoorsCount - 1)
					Doors += ",";
			}
			Doors += ")";
		}

		public string StartTime { get; private set; }
		public string EndTime { get; private set; }
		public string Doors { get; private set; }
	}
}