using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using FiresecAPI.GK;

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
			IsChecked = Employee != null && Employee.GuardZoneAccesses != null && Employee.GuardZoneAccesses.Any(x => x.ZoneUID == guardZone.UID);
		}

		internal bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

		bool _canSetZone;
		public bool CanSetZone
		{
			get { return _canSetZone; }
			set
			{
				_canSetZone = value;
				OnPropertyChanged("CanSetZone");
			}
		}

		bool _canUnSetZone;
		public bool CanUnSetZone
		{
			get { return _canUnSetZone; }
			set
			{
				_canUnSetZone = value;
				OnPropertyChanged("CanUnSetZone");
			}
		}
	}
}