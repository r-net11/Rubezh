﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentEmployeeListViewModel : EmployeeListBaseViewModel<DepartmentEmployeeListItemViewModel>
	{
		public DepartmentEmployeeListViewModel(ShortDepartment parent, Guid organisationUID)
			: base(parent.UID, organisationUID)
		{
			var chief = Employees.FirstOrDefault(x => x.Employee.UID == parent.ChiefUID);
			if (chief != null)
			{
				chief.IsChief = true;
			}
			var hrChief = Employees.FirstOrDefault(x => x.Employee.UID == parent.HRChiefUID);
			if (hrChief != null)
			{
				hrChief.IsHRChief = true;
			}
			SetChiefCommand = new RelayCommand(OnSetChief, CanSetChief);
			UnSetChiefCommand = new RelayCommand(OnUnSetChief, CanUnSetChief);
			SetHRChiefCommand = new RelayCommand(OnSetHRChief, CanSetHRChief);
			UnSetHRChiefCommand = new RelayCommand(OnUnSetHRChief, CanUnSetHRChief);
			ServiceFactory.Events.GetEvent<ChangeDepartmentChiefEvent>().Unsubscribe(OnChangeDepartmentChief);
			ServiceFactory.Events.GetEvent<ChangeDepartmentChiefEvent>().Subscribe(OnChangeDepartmentChief);
		}

		DepartmentEmployeeListItemViewModel Chief
		{
			get { return Employees.FirstOrDefault(x => x.IsChief); }
		}

		DepartmentEmployeeListItemViewModel HRChief
		{
			get { return Employees.FirstOrDefault(x => x.IsHRChief); }
		}
		
		protected override bool AddToParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, _parentUID);
		}

		protected override bool RemoveFromParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, Guid.Empty);
		}

		public override bool CanEditDepartment { get { return false; } }
		public override bool CanEditPosition { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { _parentUID } }; }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { Guid.Empty } }; }
		}

		protected override Guid GetParentUID(Employee employee)
		{
			return employee.Department != null ? employee.Department.UID : Guid.Empty;
		}

		#region Commands
		public RelayCommand SetChiefCommand { get; private set; }
		void OnSetChief()
		{
			if (Chief != null)
				Chief.IsChief = false;
			SelectedEmployee.IsChief = true;
			DepartmentHelper.SaveChief(_parentUID, SelectedEmployee.Employee.UID);
		}
		bool CanSetChief()
		{
			return SelectedEmployee != null && !SelectedEmployee.IsChief;
		}

		public RelayCommand UnSetChiefCommand { get; private set; }
		void OnUnSetChief()
		{
			Chief.IsChief = false;
			DepartmentHelper.SaveChief(_parentUID, Guid.Empty);
		}
		bool CanUnSetChief()
		{
			return SelectedEmployee != null && SelectedEmployee.IsChief;
		}

		public RelayCommand SetHRChiefCommand { get; private set; }
		void OnSetHRChief()
		{
			if (HRChief != null)
				HRChief.IsHRChief = false;
			SelectedEmployee.IsHRChief = true;
			DepartmentHelper.SaveHRChief(_parentUID, SelectedEmployee.Employee.UID);
		}
		bool CanSetHRChief()
		{
			return SelectedEmployee != null && !SelectedEmployee.IsHRChief;
		}

		public RelayCommand UnSetHRChiefCommand { get; private set; }
		void OnUnSetHRChief()
		{
			HRChief.IsHRChief = false;
			DepartmentHelper.SaveHRChief(_parentUID, Guid.Empty);
		}
		bool CanUnSetHRChief()
		{
			return SelectedEmployee != null && SelectedEmployee.IsHRChief;
		}
		#endregion

		void OnChangeDepartmentChief(Department department)
		{
			if (department.ChiefUID != Guid.Empty)
			{
				var newChief = Employees.FirstOrDefault(x => x.Employee.UID == department.ChiefUID);
				if (newChief != null)
				{
					if (Chief == null)
					{
						newChief.IsChief = true;
					}
					else if (newChief.Employee.UID != Chief.Employee.UID)
					{
						Chief.IsChief = false;
						newChief.IsChief = true;
					}
				}
			}
			else
			{
				if (Chief != null)
					Chief.IsChief = false;
			}
			if (department.HRChiefUID != Guid.Empty)
			{
				var newHRChief = Employees.FirstOrDefault(x => x.Employee.UID == department.HRChiefUID);
				if (newHRChief != null)
				{
					if (HRChief == null)
					{
						newHRChief.IsHRChief = true;
					}
					else if (newHRChief.Employee.UID != HRChief.Employee.UID)
					{
						HRChief.IsHRChief = false;
						newHRChief.IsHRChief = true;
					}
				}
			}
			else
			{
				if (HRChief != null)
					HRChief.IsHRChief = false;
			}
		}
	}

	public class DepartmentEmployeeListItemViewModel : EmployeeListItemViewModel
	{
		bool _isChief;
		public bool IsChief
		{
			get { return _isChief; }
			set
			{
				_isChief = value;
				OnPropertyChanged(() => IsChief);
			}
		}

		bool _isHRChief;
		public bool IsHRChief
		{
			get { return _isHRChief; }
			set
			{
				_isHRChief = value;
				OnPropertyChanged(() => IsHRChief);
			}
		}
	}

}

