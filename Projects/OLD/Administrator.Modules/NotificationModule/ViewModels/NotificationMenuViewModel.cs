using Infrastructure.Common.Windows.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class NotificationMenuViewModel : BaseViewModel
	{
		public NotificationMenuViewModel(NotificationViewModel context)
		{
			Context = context;
		}

		public NotificationViewModel Context { get; private set; }
	}
}