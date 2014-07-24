using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeGuardZoneViewModel : BaseViewModel
	{
		public Employee Employee { get; private set; }
		public XGuardZone GuardZone { get; private set; }

		public EmployeeGuardZoneViewModel(Employee employee, XGuardZone guardZone)
		{
			Employee = employee;
			GuardZone = guardZone;
			if (Employee != null)
			{
				if (Employee.GuardZoneAccesses == null)
					Employee.GuardZoneAccesses = new List<XGuardZoneAccess>();
			}
			if(Employee != null && Employee.GuardZoneAccesses != null)
			{
				var cardZoneAccess = Employee.GuardZoneAccesses.FirstOrDefault(x => x.ZoneUID == guardZone.UID);
				if(cardZoneAccess != null)
				{
					IsChecked = true;
					CanSetZone = cardZoneAccess.CanSet;
					CanUnSetZone = cardZoneAccess.CanReset;
				}
				
			}
			
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged(() => CanSetZone);
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged(() => CanUnSetZone);
			}
		}
	}
}