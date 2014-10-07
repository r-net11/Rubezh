using System;
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
		public DepartmentEmployeeListViewModel(DepartmentViewModel parent)
			: base(parent)
		{
			var chief = Employees.FirstOrDefault(x => x.Employee.UID == parent.Model.ChiefUID);
			if (chief != null)
			{
				chief.IsChief = true;
			}
			var hrChief = Employees.FirstOrDefault(x => x.Employee.UID == parent.Model.HRChiefUID);
			if (hrChief != null)
			{
				hrChief.IsHRChief = true;
			}
			SetChiefCommand = new RelayCommand(OnSetChief, () => CanSetChief);
			UnSetChiefCommand = new RelayCommand(OnUnSetChief, () => CanUnSetChief);
			SetHRChiefCommand = new RelayCommand(OnSetHRChief, () => CanSetHRChief);
			UnSetHRChiefCommand = new RelayCommand(OnUnSetHRChief, () => CanUnSetHRChief);
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
			return EmployeeHelper.SetDepartment(uid, _parent.UID);
		}

		protected override bool RemoveFromParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, Guid.Empty);
		}

		public override bool CanEditDepartment { get { return false; } }
		public override bool CanEditPosition { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { _parent.UID } }; }
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
			DepartmentHelper.SaveChief(_parent.UID, SelectedEmployee.Employee.UID);
			UpdateCanSet();
		}
		public bool CanSetChief
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsChief; }
		}

		public RelayCommand UnSetChiefCommand { get; private set; }
		void OnUnSetChief()
		{
			Chief.IsChief = false;
			DepartmentHelper.SaveChief(_parent.UID, Guid.Empty);
			UpdateCanSet();
		}
		public bool CanUnSetChief
		{
			get { return SelectedEmployee != null && SelectedEmployee.IsChief; }
		}

		public RelayCommand SetHRChiefCommand { get; private set; }
		void OnSetHRChief()
		{
			if (HRChief != null)
				HRChief.IsHRChief = false;
			SelectedEmployee.IsHRChief = true;
			DepartmentHelper.SaveHRChief(_parent.UID, SelectedEmployee.Employee.UID);
			UpdateCanSet();
		}
		public bool CanSetHRChief
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsHRChief; }
		}

		public RelayCommand UnSetHRChiefCommand { get; private set; }
		void OnUnSetHRChief()
		{
			HRChief.IsHRChief = false;
			DepartmentHelper.SaveHRChief(_parent.UID, Guid.Empty);
			UpdateCanSet();
		}
		public bool CanUnSetHRChief
		{
			get { return SelectedEmployee != null && SelectedEmployee.IsHRChief; }
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
			OnPropertyChanged(() => CanSetHRChief);
			OnPropertyChanged(() => CanUnSetHRChief);
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

