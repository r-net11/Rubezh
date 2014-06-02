using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class EmployeeGuardZoneViewModel : BaseViewModel
	{
		public XGuardZone GuardZone { get; private set; }

		public EmployeeGuardZoneViewModel(XGuardZone guardZone)
		{
			GuardZone = guardZone;
		}
	}
}