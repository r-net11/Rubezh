using FiresecClient;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public class UserFotterViewModel : BaseViewModel
	{
		public UserFotterViewModel()
		{
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
		}

		public string UserName
		{
			get { return FiresecManager.CurrentUser.Name; }
		}
		void OnUserChanged(UserChangedEventArgs userChangedEventArgs)
		{
			OnPropertyChanged("UserName");
		}
	}
}