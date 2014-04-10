using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public Employee Employee { get; private set; }
		public EmployeeListItem EmployeeListItem { get; private set; } 
		public bool IsEmployee { get; private set; } 

		public EmployeeDetailsViewModel(EmployeesViewModel employeesViewModel, EmployeeListItem employee = null)
		{
			EmployeesViewModel = employeesViewModel;
			if (employee == null)
			{
				Title = "Создание сотрудника";
				Employee = new Employee();
				Employee.OrganizationUID = EmployeesViewModel.Organization.UID;
				Employee.FirstName = "Новый сотрудник";
			}
			else
			{
				Title = string.Format("Свойства сотрудника: {0}", employee.FirstName);
				Employee = EmployeeHelper.GetDetails(employee.UID);
				if (Employee == null)
				{
					Employee = new Employee();
					Employee.OrganizationUID = EmployeesViewModel.Organization.UID;
				}
			}

			IsEmployee = Employee.Type == PersonType.Employee;
			if (IsEmployee)
			{
				InitializePositions();
				InitializeDepartments();
			}

			AdditionalColumns = new List<AdditionalColumnViewModel>();
			if (EmployeesViewModel.AdditionalColumnTypes.IsNotNullOrEmpty())
			{
				foreach (var column in Employee.AdditionalColumns)
				{
					AdditionalColumns.Add(new AdditionalColumnViewModel(column));
				}
				SelectedAdditionalColumn = AdditionalColumns.FirstOrDefault();
			}
			CopyProperties();
		}

		private void InitializeDepartments()
		{
			Departments = new List<SelectationDepartmentViewModel>();
			var departments = DepartmentHelper.GetByOrganization(Employee.OrganizationUID);
			if (departments != null)
			{
				foreach (var department in departments)
				{
					Departments.Add(new SelectationDepartmentViewModel(department));
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
			var selectedDepartment = Departments.FirstOrDefault(x => x.Department.UID == Employee.DepartmentUID);
			if (selectedDepartment != null)
				selectedDepartment.IsChecked = true;
		}

		private void InitializePositions()
		{
			var positions = PositionHelper.GetByOrganization(Employee.OrganizationUID);
			if (positions == null)
				Positions = new List<Position>();
			else
				Positions = positions.ToList();

			if (Employee.Position == null)
			{
				SelectedPosition = Positions.FirstOrDefault();
			}
			else
			{
				SelectedPosition = Positions.FirstOrDefault(x => x.UID == Employee.Position.UID);
				if (SelectedPosition == null)
					SelectedPosition = Positions.FirstOrDefault();
			}
		}

		List<SelectationDepartmentViewModel> GetAllChildren(SelectationDepartmentViewModel department)
		{
			var result = new List<SelectationDepartmentViewModel>();
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

		void SetChildren(SelectationDepartmentViewModel department)
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

		List<SelectationDepartmentViewModel> departments;
		public List<SelectationDepartmentViewModel> Departments
		{
			get { return departments; }
			private set
			{
				departments = value;
				OnPropertyChanged(() => Departments);
			}
		}

		SelectationDepartmentViewModel[] rootDepartments;
		public SelectationDepartmentViewModel[] RootDepartments
		{
			get { return rootDepartments; }
			set
			{
				rootDepartments = value;
				OnPropertyChanged(() => RootDepartments);
			}
		}

		public void CopyProperties()
		{
			FirstName = Employee.FirstName;
			SecondName = Employee.SecondName;
			LastName = Employee.LastName;
		}

		public bool HasAdditionalColumns
		{
			get { return AdditionalColumns.IsNotNullOrEmpty(); }
		}

		public List<Position> Positions { get; private set; }

		Position _selectedPosition;
		public Position SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged(() => SelectedPosition);
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				if (_firstName != value)
				{
					_firstName = value;
					OnPropertyChanged(() => FirstName);
				}
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				if (_secondName != value)
				{
					_secondName = value;
					OnPropertyChanged(() => SecondName);
				}
			}
		}

		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				if (_lastName != value)
				{
					_lastName = value;
					OnPropertyChanged(() => LastName);
				}
			}
		}

		List<AdditionalColumnViewModel> additionalColumns;
		public List<AdditionalColumnViewModel> AdditionalColumns
		{
			get { return additionalColumns; }
			set
			{
				additionalColumns = value;
				OnPropertyChanged(() => AdditionalColumns);
			}
		}

		AdditionalColumnViewModel selectedAdditionalColumn;
		public AdditionalColumnViewModel SelectedAdditionalColumn
		{
			get { return selectedAdditionalColumn; }
			set
			{
				selectedAdditionalColumn = value;
				OnPropertyChanged(() => SelectedAdditionalColumn);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		protected override bool Save()
		{
			if (EmployeesViewModel.Employees.Any(x => x.EmployeeListItem.FirstName == FirstName && x.EmployeeListItem.LastName == LastName && x.EmployeeListItem.UID != Employee.UID))
			{
				MessageBoxService.ShowWarning("Имя и фамилия сотрудника совпадает с введеннымы ранее");
				return false;
			}

			Employee.FirstName = FirstName;
			Employee.SecondName = SecondName;
			Employee.LastName = LastName;
			if (SelectedPosition != null)
				Employee.Position = SelectedPosition;
			Employee.AdditionalColumns = (from x in AdditionalColumns select x.AdditionalColumn).ToList();

			EmployeeListItem = new EmployeeListItem
			{
				UID = Employee.UID,
				Cards = Employee.Cards,
				FirstName = Employee.FirstName,
				SecondName = Employee.SecondName,
				LastName = Employee.LastName,
				Type = Employee.Type,
				Appointed = Employee.Appointed.ToString("d MMM yyyy"),
				Dismissed = Employee.Dismissed.ToString("d MMM yyyy"),
			};

			if (IsEmployee)
			{
				var selectedDepartment = Departments.FirstOrDefault(x => x.IsChecked);
				if (selectedDepartment != null)
				{
					Employee.DepartmentUID = selectedDepartment.Department.UID;
					EmployeeListItem.DepartmentName = selectedDepartment.Department.Name;
				}
				EmployeeListItem.PositionName = SelectedPosition.Name;
			}
			return true;
		}
	}

	public class SelectationDepartmentViewModel : TreeNodeViewModel<SelectationDepartmentViewModel>
	{
		public SelectationDepartmentViewModel(Department department)
		{
			Department = department;
		}

		public Department Department { get; private set; }
		
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
				if(value)
					Trace.WriteLine(Department.Name);
			}
		}
	}
}