using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFiltersMenuViewModel : BaseViewModel
	{
		public OpcDaTagFiltersMenuViewModel(OpcDaTagFiltersViewModel contex)
		{
			Context = contex;
		}

		public OpcDaTagFiltersViewModel Context { get; private set; }
	}
}
