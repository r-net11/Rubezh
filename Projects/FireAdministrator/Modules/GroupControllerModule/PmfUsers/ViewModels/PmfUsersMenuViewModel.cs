using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	class PmfUsersMenuViewModel : BaseViewModel
	{
		public PmfUsersMenuViewModel(PmfUsersViewModel context)
		{
			Context = context;
		}

		public PmfUsersViewModel Context { get; private set; }
	}
}
