using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using SKDModule.Events;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class DepartmentEmployeeListViewModel : EmployeeListBaseViewModel<DepartmentEmployeeListItemViewModel>
	{
		public DepartmentEmployeeListViewModel(DepartmentViewModel parent, bool isWithDeleted)
			: base(parent, isWithDeleted)
		{
			var chief = Employees.FirstOrDefault(x => x.Employee.UID == parent.Model.ChiefUID);
			if (chief != null)
			{
				chief.IsChief = true;
			}
			SetChiefCommand = new RelayCommand(OnSetChief, () => CanSetChief);
			UnSetChiefCommand = new RelayCommand(OnUnSetChief, () => CanUnSetChief);
			ServiceFactoryBase.Events.GetEvent<ChangeDepartmentChiefEvent>().Unsubscribe(OnChangeDepartmentChief);
			ServiceFactoryBase.Events.GetEvent<ChangeDepartmentChiefEvent>().Subscribe(OnChangeDepartmentChief);
		}

		DepartmentEmployeeListItemViewModel Chief
		{
			get { return Employees.FirstOrDefault(x => x.IsChief); }
		}

		protected override bool AddToParent(ShortEmployee employee)
		{
			return EmployeeHelper.SetDepartment(employee, Parent.UID);
		}

		protected override bool RemoveFromParent(ShortEmployee employee)
		{
			return EmployeeHelper.SetDepartment(employee, Guid.Empty);
		}

		public override bool CanEditDepartment { get { return false; } }
		public override bool CanEditPosition { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { Parent.UID }, OrganisationUIDs = new List<Guid>{ Parent.OrganisationUID }, LogicalDeletationType = IsWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active }; }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { Guid.Empty }, OrganisationUIDs = new List<Guid> { Parent.OrganisationUID }, WithDeletedDepartments = true }; }
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
			DepartmentHelper.SaveChief(Parent.UID, SelectedEmployee.Employee.UID, Parent.Name);
			UpdateCanSet();
		}
		public bool CanSetChief
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsDeleted && !SelectedEmployee.IsChief && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Departments_Etit) && !Parent.IsDeleted; }
		}

		public RelayCommand UnSetChiefCommand { get; private set; }
		void OnUnSetChief()
		{
			Chief.IsChief = false;
			DepartmentHelper.SaveChief(Parent.UID, Guid.Empty, Parent.Name);
			UpdateCanSet();
		}
		public bool CanUnSetChief
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsDeleted && SelectedEmployee.IsChief && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Departments_Etit) && !Parent.IsDeleted; }
		}

		protected override void Update()
		{
			base.Update();
			UpdateCanSet();
		}

		void UpdateCanSet()
		{
			OnPropertyChanged(() => CanSetChief);
			OnPropertyChanged(() => CanUnSetChief);
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
	}
}

