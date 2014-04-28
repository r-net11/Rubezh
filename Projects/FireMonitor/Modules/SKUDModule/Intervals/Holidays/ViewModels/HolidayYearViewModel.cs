using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class HolidayYearViewModel : BaseViewModel
	{
		public int Year { get; private set; }

		public HolidayYearViewModel(int year)
		{
			Year = year;
		}
	}
}