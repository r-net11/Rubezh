using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class PmfUserViewModel : BaseViewModel
	{
		public GKUser User { get; private set; }
		public PmfUserViewModel(GKUser user)
		{
			User = user;
		}

		public void Update(GKUser user)
		{
			User = user;
			OnPropertyChanged(() => User);
		}
	}
}
