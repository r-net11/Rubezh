using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class OPCItemViewModel : BaseViewModel
	{
		public OPCItemViewModel(GKBase objects )
		{ 
			Object = objects;
		}

		public GKBase Object { get; private set; }
	}
}
