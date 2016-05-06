using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SelectableDoorDayInterval : BaseViewModel
	{
		public SKDDoorDayInterval DayInterval { get; private set; }

		public SelectableDoorDayInterval(SKDDoorDayInterval dayInterval)
		{
			DayInterval = dayInterval;
		}
	}
}