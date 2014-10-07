using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
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