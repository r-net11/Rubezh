using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
{
	public class UserFotterViewModel : BaseViewModel
	{
		public UserFotterViewModel()
		{
			FiresecManager.UserChanged += OnUserChanged;
		}

		public string UserName
		{
			get { return FiresecManager.CurrentUser.Name; }
		}
		private void OnUserChanged()
		{
			OnPropertyChanged("UserName");
		}

	}
}
