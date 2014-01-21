using Infrastructure.Common.Windows.ViewModels;

namespace SkudModule.ViewModels
{
	public class HolidaysMenuViewModel : BaseViewModel
	{
		public HolidaysMenuViewModel(HolidaysViewModel context)
		{
			Context = context;
		}

		public HolidaysViewModel Context { get; private set; }
	}
}