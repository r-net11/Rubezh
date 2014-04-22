using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class EmployeeFilterViewModel : FilterBaseViewModel<EmployeeFilter>
	{
		public EmployeeFilterViewModel(EmployeeFilter filter)
			: base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			Departments = new List<FilterDepartmentViewModel>();
			var departments = DepartmentHelper.GetList(null);
			if (departments != null)
			{
				foreach (var department in departments)
				{
					Departments.Add(new FilterDepartmentViewModel(department, this));
				}
				RootDepartments = Departments.Where(x => x.Department.ParentDepartmentUID == null).ToArray();
				if (RootDepartments.IsNotNullOrEmpty())
				{
					foreach (var rootDepartment in RootDepartments)
					{
						SetChildren(rootDepartment);	
					}
				}
			}
			
			Positions = new CheckBoxItemList<FilterPositionViewModel>();
			var positions = PositionHelper.Get(null);
			if (positions != null)
				foreach (var position in positions)
					Positions.Add(new FilterPositionViewModel(position));

			HasManyPersonTypes = FiresecManager.CurrentUser.IsEmployeesAllowed && FiresecManager.CurrentUser.IsGuestsAllowed;
			if (HasManyPersonTypes)
			{
				if (Filter.PersonType == PersonType.Guest)
					IsGuestsAllowed = true;
				else
					IsEmployeesAllowed = true;
			}
			AvailableOrganisations = new ObservableCollection<FilterOrganisationViewModel>();
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			foreach (var organisation in organisations)
			{
				AvailableOrganisations.Add(new FilterOrganisationViewModel(organisation));
			}
			var selectedOrganisation = AvailableOrganisations.FirstOrDefault(x => x.Organisation.UID == Filter.OrganisationUID);
			if (selectedOrganisation != null)
				selectedOrganisation.IsChecked = true;
			else
				AvailableOrganisations.FirstOrDefault().IsChecked = true;
		}

		protected override void Update()
		{
			base.Update();
			Departments.ForEach(x => x.IsChecked = false);
			Positions.Items.ForEach(x => x.IsChecked = false);
			Departments.ForEach(x =>
			{
				if (Filter.DepartmentUIDs.Any(y => y == x.Department.UID))
					x.IsChecked = true;
			});
			Positions.Items.ForEach(x =>
			{
				if (Filter.PositionUIDs.Any(y => y == x.Position.UID))
					x.IsChecked = true;
			});
		}

		List<FilterDepartmentViewModel> GetAllChildren(FilterDepartmentViewModel department)
		{
			var result = new List<FilterDepartmentViewModel>();
			if (department.Department.ChildDepartmentUIDs.Count == 0)
				return result;
			Departments.ForEach(x =>
			{
				if (department.Department.ChildDepartmentUIDs.Contains(x.Department.UID))
				{
					result.Add(x);
					result.AddRange(GetAllChildren(x));
				}
			});
			return result;
		}

		void SetChildren(FilterDepartmentViewModel department)
		{
			if (department.Department.ChildDepartmentUIDs.Count == 0)
				return;
			var children = Departments.Where(x => department.Department.ChildDepartmentUIDs.Any(y => y == x.Department.UID));
			foreach (var child in children)
			{
				department.AddChild(child);
				SetChildren(child);
			}
		}

		public bool HasManyPersonTypes { get; private set; }

		public void SetChildDepartmentsChecked(FilterDepartmentViewModel department)
		{
			GetAllChildren(department).ForEach(x => x.IsChecked = true);
		}

		
		List<FilterDepartmentViewModel> departments;
		public List<FilterDepartmentViewModel> Departments
		{
			get { return departments; }
			private set
			{
				departments = value;
				OnPropertyChanged(()=>Departments);
			}
		}

		FilterDepartmentViewModel[] rootDepartments;
		public FilterDepartmentViewModel[] RootDepartments
		{
			get { return rootDepartments; }
			set
			{
				rootDepartments = value;
				OnPropertyChanged(() => RootDepartments);
			}
		}

		public CheckBoxItemList<FilterPositionViewModel> Positions { get; private set; }
		
		public ObservableCollection<FilterOrganisationViewModel> AvailableOrganisations { get; private set; }

		bool _isEmployeesAllowed;
		public bool IsEmployeesAllowed
		{
			get { return _isEmployeesAllowed; }
			set
			{
				_isEmployeesAllowed = value;
				OnPropertyChanged(() => IsEmployeesAllowed);
			}
		}

		bool _isGuestsAllowed;
		public bool IsGuestsAllowed
		{
			get { return _isGuestsAllowed; }
			set
			{
				_isGuestsAllowed = value;
				OnPropertyChanged(() => IsGuestsAllowed);
			}
		}

		protected override bool Save()
		{
			base.Save();
			Filter.PositionUIDs = new List<Guid>();
			foreach (var position in Positions.Items)
			{
				if (position.IsChecked)
					Filter.PositionUIDs.Add(position.Position.UID);
			};
			Filter.DepartmentUIDs = new List<Guid>();
			foreach (var Department in Departments)
			{
				if (Department.IsChecked)
					Filter.DepartmentUIDs.Add(Department.Department.UID);
			};
			Filter.OrganisationUIDs = new List<Guid>();
			var selectedOrganisation = AvailableOrganisations.FirstOrDefault(x => x.IsChecked);
			if (selectedOrganisation != null)
			{
				Filter.OrganisationUID = selectedOrganisation.Organisation.UID;
				Filter.OrganisationUIDs.Add(selectedOrganisation.Organisation.UID);
			}
			
			if (IsGuestsAllowed)
				Filter.PersonType = PersonType.Guest;
			else
				Filter.PersonType = PersonType.Employee;
			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new EmployeeFilter();
			Update();
		}
	}

	public class FilterDepartmentViewModel : TreeNodeViewModel<FilterDepartmentViewModel>
	{
		public FilterDepartmentViewModel(ShortDepartment department, EmployeeFilterViewModel employeeFilterViewModel)
		{
			Department = department;
			DepartmentCheckedCommand = new RelayCommand(OnDepartmentChecked);
			EmployeeFilterViewModel = employeeFilterViewModel;
		}

		public ShortDepartment Department { get; private set; }
		EmployeeFilterViewModel EmployeeFilterViewModel;

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

		public RelayCommand DepartmentCheckedCommand { get; private set; }
		void OnDepartmentChecked()
		{
			if (IsChecked)
				EmployeeFilterViewModel.SetChildDepartmentsChecked(this);
		}
	}

	public class FilterPositionViewModel : CheckBoxItemViewModel
	{
		public FilterPositionViewModel(ShortPosition position)
		{
			Position = position;
		}

		public ShortPosition Position { get; private set; }
	}
}