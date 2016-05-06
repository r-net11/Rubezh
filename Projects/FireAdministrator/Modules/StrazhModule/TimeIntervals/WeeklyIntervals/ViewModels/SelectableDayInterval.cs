using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SelectableDayInterval : BaseViewModel
	{
		public SKDDayInterval DayInterval { get; private set; }

		public SelectableDayInterval(SKDDayInterval dayInterval)
		{
			DayInterval = dayInterval;
		}
	}
}