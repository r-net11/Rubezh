﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public Employee Employee { get; private set; }
		public ShortEmployee ShortEmployee { get; private set; } 
		public bool IsEmployee { get; private set; }
		SelectDepartmentViewModel SelectDepartmentViewModel;
		SelectPositionViewModel SelectPositionViewModel;

		public EmployeeDetailsViewModel(EmployeesViewModel employeesViewModel, ShortEmployee employee = null)
		{
			EmployeesViewModel = employeesViewModel;
			IsEmployee = EmployeesViewModel.PersonType == PersonType.Employee;
			if (employee == null)
			{
				Employee = new Employee();
				Employee.FirstName = "Новый сотрудник";
				if (IsEmployee)
				{
					Title = "Добавить сотрудника";
					Employee.FirstName = "Новый сотрудник";
				}
				else
				{
					Title = "Добавить посетителя";
					Employee.FirstName = "Новый посетитель";
				}
			}
			else
			{
				Title = string.Format("Свойства сотрудника: {0}", employee.FirstName);
				Employee = EmployeeHelper.GetDetails(employee.UID);
				if (Employee == null)
					Employee = new Employee();
				if (IsEmployee)
					Title = string.Format("Свойства сотрудника: {0}", employee.FirstName);
				else
					Title = string.Format("Свойства посетителя: {0}", employee.FirstName);
			}
			CopyProperties();
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			RemoveDepartmentCommand = new RelayCommand(OnRemoveDepartment);
			SelectPositionCommand = new RelayCommand(OnSelectPosition);
			RemovePositionCommand = new RelayCommand(OnRemovePosition);
		}

		public void CopyProperties()
		{
			FirstName = Employee.FirstName;
			SecondName = Employee.SecondName;
			LastName = Employee.LastName;
			if (IsEmployee)
			{
				SelectPositionViewModel = new SelectPositionViewModel(Employee);
				SelectedPosition = SelectPositionViewModel.SelectedPosition;
				SelectDepartmentViewModel = new SelectDepartmentViewModel(Employee);
				SelectedDepartment = SelectDepartmentViewModel.SelectedDepartment;
			}
			TextColumns = new List<TextColumnViewModel>();
			GraphicsColumns = new List<IGraphicsColumnViewModel>();
			GraphicsColumns.Add(new PhotoColumnViewModel(Employee.Photo));
			SelectedGraphicsColumn = GraphicsColumns.FirstOrDefault();
			var graphicsColumnTypes = EmployeesViewModel.AdditionalColumnTypes.Where(x => x.DataType == AdditionalColumnDataType.Graphics);
			HasAdditionalGraphicsColumns = graphicsColumns != null && graphicsColumns.Count > 0;
			if (EmployeesViewModel.AdditionalColumnTypes.IsNotNullOrEmpty())
			{
				foreach (var column in Employee.AdditionalColumns)
				{
					if (column.AdditionalColumnType.DataType == AdditionalColumnDataType.Text)
						TextColumns.Add(new TextColumnViewModel(column));
					if (column.AdditionalColumnType.DataType == AdditionalColumnDataType.Graphics)
						GraphicsColumns.Add(new GraphicsColumnViewModel(column));
				}
			}
			HasAdditionalGraphicsColumns = GraphicsColumns.Count > 1;
			GraphicsColumnsTabItemName = HasAdditionalGraphicsColumns ? "Фото и графические данные" : "Фото";
		}

		SelectationDepartmentViewModel selectedDepartment;
		public SelectationDepartmentViewModel SelectedDepartment
		{
			get { return selectedDepartment; }
			private set
			{
				selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
				OnPropertyChanged(() => HasSelectedDepartment);
			}
		}

		public bool HasSelectedDepartment 
		{
			get { return SelectedDepartment != null; }
		}
		
		SelectationPositionViewModel _selectedPosition;
		public SelectationPositionViewModel SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged(() => SelectedPosition);
				OnPropertyChanged(() => HasSelectedPosition);
			}
		}

		public bool HasSelectedPosition
		{
			get { return SelectedPosition != null; }
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

		public bool HasTextColumns
		{
			get { return TextColumns.IsNotNullOrEmpty(); }
		}

		List<TextColumnViewModel> textColumns;
		public List<TextColumnViewModel> TextColumns
		{
			get { return textColumns; }
			set
			{
				textColumns = value;
				OnPropertyChanged(() => TextColumns);
			}
		}

		TextColumnViewModel selectedTextColumn;
		public TextColumnViewModel SelectedTextColumn
		{
			get { return selectedTextColumn; }
			set
			{
				selectedTextColumn = value;
				OnPropertyChanged(() => SelectedTextColumn);
			}
		}

		public bool HasAdditionalGraphicsColumns { get; private set; }
		public string GraphicsColumnsTabItemName { get; private set; }
		
		List<IGraphicsColumnViewModel> graphicsColumns;
		public List<IGraphicsColumnViewModel> GraphicsColumns
		{
			get { return graphicsColumns; }
			set
			{
				graphicsColumns = value;
				OnPropertyChanged(() => GraphicsColumns);
			}
		}

		IGraphicsColumnViewModel selectedGraphicsColumn;
		public IGraphicsColumnViewModel SelectedGraphicsColumn
		{
			get { return selectedGraphicsColumn; }
			set
			{
				selectedGraphicsColumn = value;
				OnPropertyChanged(() => SelectedGraphicsColumn);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		void OnSelectDepartment()
		{
			if (DialogService.ShowModalWindow(SelectDepartmentViewModel))
			{
				SelectedDepartment = SelectDepartmentViewModel.SelectedDepartment;
			}
		}

		public RelayCommand RemoveDepartmentCommand { get; private set; }
		void OnRemoveDepartment()
		{
			SelectedDepartment = null;
		}

		public RelayCommand SelectPositionCommand { get; private set; }
		void OnSelectPosition()
		{
			if (DialogService.ShowModalWindow(SelectPositionViewModel))
			{
				SelectedPosition = SelectPositionViewModel.SelectedPosition;
			}
		}

		public RelayCommand RemovePositionCommand { get; private set; }
		void OnRemovePosition()
		{
			SelectedPosition = null;
		}

		protected override bool Save()
		{
			if (EmployeesViewModel.Employees.Any(x => x.ShortEmployee.FirstName == FirstName && x.ShortEmployee.LastName == LastName && x.ShortEmployee.UID != Employee.UID))
			{
				MessageBoxService.ShowWarning("Имя и фамилия сотрудника совпадает с введеннымы ранее");
				return false;
			}
			Employee.FirstName = FirstName;
			Employee.SecondName = SecondName;
			Employee.LastName = LastName;
			Employee.OrganizationUID = EmployeesViewModel.Organization.UID;
			Employee.AdditionalColumns = (from x in TextColumns select x.AdditionalColumn).ToList();
			foreach (var item in GraphicsColumns)
			{
				var graphicsColumnViewModel = item as GraphicsColumnViewModel;
				if (graphicsColumnViewModel != null)
				{
					Employee.AdditionalColumns.Add(graphicsColumnViewModel.AdditionalColumn);
					continue;
				}
				var photoColumnViewModel = item as PhotoColumnViewModel;
				if (photoColumnViewModel != null)
				{
					Employee.Photo = photoColumnViewModel.Photo;
				}
			}

			ShortEmployee = new ShortEmployee
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
				if (SelectedDepartment == null)
				{
					MessageBoxService.ShowWarning("Выберите отдел");
					return false;
				}
				if (SelectedPosition == null)
				{
					MessageBoxService.ShowWarning("Выберите должность");
					return false;
				}
				Employee.DepartmentUID = SelectedDepartment.Department.UID;
				ShortEmployee.DepartmentName = SelectedDepartment.Department.Name;
				Employee.Position = SelectedPosition.Position;
				ShortEmployee.PositionName = SelectedPosition.Name;
			}
			
			return EmployeeHelper.Save(Employee);
		}
	}
}