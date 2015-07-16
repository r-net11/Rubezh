using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class OPCMenuViewModel: BaseViewModel
	{
		public OPCMenuViewModel(OPCsViewModel context)
		{
			Context = context;
		}
		public OPCsViewModel Context { get; private set; }
	}
}
