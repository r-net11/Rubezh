using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

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
