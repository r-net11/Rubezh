﻿using System.Globalization;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortEmployee>
	{
		Organisation _organisation;
		PersonType _personType;
		bool _isNew;
		public ObservableCollection<Gender> Genders { get; private set; }
		public ObservableCollection<EmployeeDocumentType> DocumentTypes { get; private set; }
		bool _isWithDeleted;

		public bool Initialize(Organisation organisation, ShortEmployee employee, ViewPartViewModel parentViewModel)
		{
			var employeesViewModel = (parentViewModel as EmployeesViewModel);
			return Initialize(organisation.UID, employee, employeesViewModel.PersonType, isWithDeleted: employeesViewModel.IsWithDeleted);
		}

		public bool Initialize(Guid organisationUID, ShortEmployee employee, PersonType personType, bool canEditDepartment = true, bool canEditPosition = true, bool isWithDeleted = false)
		{
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			SelectPositionCommand = new RelayCommand(OnSelectPosition);
			SelectScheduleCommand = new RelayCommand(OnSelectSchedule);
			SelectEscortCommand = new RelayCommand(OnSelectEscort);

			CanEditDepartment = canEditDepartment;
			_canEditPosition = canEditPosition;
			_isWithDeleted = isWithDeleted;

			Genders = new ObservableCollection<Gender>();
			foreach (Gender item in Enum.GetValues(typeof(Gender)))
			{
				Genders.Add(item);
			}

			DocumentTypes = new ObservableCollection<EmployeeDocumentType>();
			foreach (EmployeeDocumentType item in Enum.GetValues(typeof(EmployeeDocumentType)))
			{
				DocumentTypes.Add(item);
			}

			_organisation = OrganisationHelper.GetSingle(organisationUID);
			_personType = personType;
			IsEmployee = _personType == PersonType.Employee;
			_isNew = employee == null;
			if (_isNew)
			{
				Employee = new Employee
				{
					OrganisationUID = organisationUID,
					RemovalDate = DateTime.Now,
					ScheduleStartDate = DateTime.Now
				};
				Title = IsEmployee ? "Добавить сотрудника" : "Добавить посетителя";

			}
			else
			{
				if (employee != null)
				{
					Employee = EmployeeHelper.GetDetails(employee.UID) ?? new Employee();
					Title = string.Format(IsEmployee ? "Свойства сотрудника: {0}" : "Свойства посетителя: {0}", employee.FIO);
				}
			}
			CopyProperties();
			return true;
		}

		void CopyProperties()
		{
			FirstName = Employee.FirstName;
			SecondName = Employee.SecondName;
			LastName = Employee.LastName;
			DocumentNumber = Employee.DocumentNumber;
			BirthDate = Employee.BirthDate.HasValue ? Employee.BirthDate.Value.Date : default(DateTime?);
			BirthPlace = Employee.BirthPlace;
			GivenBy = Employee.DocumentGivenBy;
			GivenDate = Employee.DocumentGivenDate.HasValue ? Employee.DocumentGivenDate.Value.Date : default(DateTime?);
			Gender = Employee.Gender;
			ValidTo = Employee.DocumentValidTo.HasValue ? Employee.DocumentValidTo.Value.Date : default(DateTime?);
			Citizenship = Employee.Citizenship;
			DocumentType = Employee.DocumentType;
			Phone = Employee.Phone;
			if (IsEmployee)
			{
				SelectedPosition = Employee.Position;
				SelectedSchedule = Employee.Schedule;
				ScheduleStartDate = Employee.ScheduleStartDate;
				CredentialsStartDate = Employee.CredentialsStartDate;
				TabelNo = Employee.TabelNo;
				IsOrganisationChief = _organisation.ChiefUID == Employee.UID;
				IsOrganisationHRChief = _organisation.HRChiefUID == Employee.UID;
			}
			else
			{
				SelectedEscort = EmployeeHelper.GetSingleShort(Employee.EscortUID);
				Description = Employee.Description;
			}
			SelectedDepartment = Employee.Department;
			TextColumns = new List<TextColumnViewModel>();
			GraphicsColumns = new List<IGraphicsColumnViewModel> {new PhotoColumnViewModel(Employee.Photo)};
			SelectedGraphicsColumn = GraphicsColumns.FirstOrDefault();
			var additionalColumnTypes = AdditionalColumnTypeHelper.GetByOrganisation(Employee.OrganisationUID);
			if (additionalColumnTypes != null && additionalColumnTypes.Any())
			{
				foreach (var columnType in additionalColumnTypes)
				{
					var columnValue = Employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnType.UID == columnType.UID);
					if (columnType.DataType == AdditionalColumnDataType.Text)
						TextColumns.Add(new TextColumnViewModel(columnType, Employee, columnValue));
					if (columnType.DataType == AdditionalColumnDataType.Graphics)
						GraphicsColumns.Add(new GraphicsColumnViewModel(columnType, Employee, columnValue));
				}
			}
			HasAdditionalGraphicsColumns = GraphicsColumns.Count > 1;
			GraphicsColumnsTabItemName = HasAdditionalGraphicsColumns ? "Фото и графические данные" : "Фото";
		}

		public Employee Employee { get; private set; }
		public ShortEmployee Model
		{
			get
			{
				var result = new ShortEmployee
				{
					UID = Employee.UID,
					FirstName = FirstName,
					SecondName = SecondName,
					LastName = LastName,
					Type = Employee.Type,
					CredentialsStartDate = CredentialsStartDate.ToString(CultureInfo.CurrentUICulture),
					TextColumns = new List<TextColumn>(),
					Phone = Employee.Phone,
					Description = Employee.Description,
					OrganisationName = _organisation.Name,
				};
				if (SelectedDepartment != null)
					result.DepartmentName = SelectedDepartment.Name;
				if (SelectedPosition != null)
					result.PositionName = SelectedPosition.Name;
				foreach (var item in Employee.AdditionalColumns.Where(x => x.AdditionalColumnType.DataType == AdditionalColumnDataType.Text))
				{
					result.TextColumns.Add(new TextColumn { ColumnTypeUID = item.AdditionalColumnType.UID, Text = item.TextData });
				}
				return result;
			}
		}
		public bool IsEmployee { get; private set; }
		public bool CanEditDepartment { get; private set; }

		bool _canEditPosition;
		public bool CanEditPosition
		{
			get { return IsEmployee && _canEditPosition ; }
		}

		ShortDepartment _selectedDepartment;
		public ShortDepartment SelectedDepartment
		{
			get { return _selectedDepartment; }
			private set
			{
				_selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
				OnPropertyChanged(() => HasSelectedDepartment);
			}
		}

		public bool HasSelectedDepartment
		{
			get { return SelectedDepartment != null && !SelectedDepartment.IsDeleted; }
		}

		ShortEmployee _selectedEscort;
		public ShortEmployee SelectedEscort
		{
			get { return _selectedEscort; }
			private set
			{
				_selectedEscort = value;
				OnPropertyChanged(() => SelectedEscort);
				OnPropertyChanged(() => HasSelectedEscort);
			}
		}

		public bool HasSelectedEscort
		{
			get { return SelectedEscort != null && !SelectedEscort.IsDeleted; }
		}

		ShortPosition _selectedPosition;
		public ShortPosition SelectedPosition
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
			get { return SelectedPosition != null && !SelectedPosition.IsDeleted; }
		}

		ShortSchedule _selectedSchedule;
		public ShortSchedule SelectedSchedule
		{
			get { return _selectedSchedule; }
			private set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
				OnPropertyChanged(() => HasSelectedSchedule);
				OnPropertyChanged(() => ScheduleString);
			}
		}
		public bool HasSelectedSchedule
		{
			get { return SelectedSchedule != null && !SelectedSchedule.IsDeleted; }
		}

		DateTime _scheduleStartDate;
		public DateTime ScheduleStartDate
		{
			get { return _scheduleStartDate; }
			set
			{
				_scheduleStartDate = value;
				OnPropertyChanged(() => ScheduleStartDate);
				OnPropertyChanged(() => ScheduleString);
			}
		}

		public string ScheduleString
		{
			get {
				return HasSelectedSchedule ? string.Format("{0} с {1}", SelectedSchedule.Name, ScheduleStartDate.ToString("dd/MM/yyyy")) : string.Empty;
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				if (_firstName == value) return;
				_firstName = value;
				OnPropertyChanged(() => FirstName);
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				if (_secondName == value) return;
				_secondName = value;
				OnPropertyChanged(() => SecondName);
			}
		}

		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				if (_lastName == value) return;
				_lastName = value;
				OnPropertyChanged(() => LastName);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description == value) return;
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				if (_phone == value) return;
				_phone = value;
				OnPropertyChanged(() => Phone);
			}
		}

		DateTime _credentialsStartDate;
		public DateTime CredentialsStartDate
		{
			get { return _credentialsStartDate; }
			set
			{
				if (_credentialsStartDate == value) return;
				_credentialsStartDate = value;
				OnPropertyChanged(() => CredentialsStartDate);
			}
		}

		string _tabelNo;
		public string TabelNo
		{
			get { return _tabelNo; }
			set
			{
				if (_tabelNo == value) return;
				_tabelNo = value;
				OnPropertyChanged(() => TabelNo);
			}
		}

		bool _isOrganisationChief;
		public bool IsOrganisationChief
		{
			get { return _isOrganisationChief; }
			set
			{
				if (_isOrganisationChief == value) return;
				_isOrganisationChief = value;
				OnPropertyChanged(() => IsOrganisationChief);
			}
		}

		bool _isOrganisationHRChief;
		public bool IsOrganisationHRChief
		{
			get { return _isOrganisationHRChief; }
			set
			{
				if (_isOrganisationHRChief == value) return;
				_isOrganisationHRChief = value;
				OnPropertyChanged(() => IsOrganisationHRChief);
			}
		}

		#region Document
		string _number;
		public string DocumentNumber
		{
			get { return _number; }
			set
			{
				_number = value;
				OnPropertyChanged(() => DocumentNumber);
			}
		}

		DateTime? _birthDate;
		public DateTime? BirthDate
		{
			get { return _birthDate; }
			set
			{
				_birthDate = value;
				OnPropertyChanged(() => BirthDate);
			}
		}

		string _birthPlace;
		public string BirthPlace
		{
			get { return _birthPlace; }
			set
			{
				_birthPlace = value;
				OnPropertyChanged(() => BirthPlace);
			}
		}

		DateTime? _givenDate;
		public DateTime? GivenDate
		{
			get { return _givenDate; }
			set
			{
				_givenDate = value;
				OnPropertyChanged(() => GivenDate);
			}
		}

		string _givenBy;
		public string GivenBy
		{
			get { return _givenBy; }
			set
			{
				_givenBy = value;
				OnPropertyChanged(() => GivenBy);
			}
		}

		Gender? _gender;
		public Gender? Gender
		{
			get { return _gender; }
			set
			{
				_gender = value;
				OnPropertyChanged(() => Gender);
			}
		}

		DateTime? _validTo;
		public DateTime? ValidTo
		{
			get { return _validTo; }
			set
			{
				_validTo = value;
				OnPropertyChanged(() => ValidTo);
			}
		}

		string _departmentCode;
		public string DepartmentCode
		{
			get { return _departmentCode; }
			set
			{
				_departmentCode = value;
				OnPropertyChanged(() => DepartmentCode);
			}
		}

		string _citizenship;
		public string Citizenship
		{
			get { return _citizenship; }
			set
			{
				_citizenship = value;
				OnPropertyChanged(() => Citizenship);
			}
		}

		EmployeeDocumentType? _documentType;
		public EmployeeDocumentType? DocumentType
		{
			get { return _documentType; }
			set
			{
				_documentType = value;
				OnPropertyChanged(() => DocumentType);
				OnPropertyChanged(() => DocumentTypeString);
			}
		}

		public string DocumentTypeString
		{
			get { return DocumentType.ToDescription(); }
		}
		#endregion

		#region AdditionalColumns
		public bool HasAdditionalGraphicsColumns { get; private set; }
		public string GraphicsColumnsTabItemName { get; private set; }

		List<IGraphicsColumnViewModel> _graphicsColumns;
		public List<IGraphicsColumnViewModel> GraphicsColumns
		{
			get { return _graphicsColumns; }
			set
			{
				_graphicsColumns = value;
				OnPropertyChanged(() => GraphicsColumns);
			}
		}

		IGraphicsColumnViewModel _selectedGraphicsColumn;
		public IGraphicsColumnViewModel SelectedGraphicsColumn
		{
			get { return _selectedGraphicsColumn; }
			set
			{
				_selectedGraphicsColumn = value;
				OnPropertyChanged(() => SelectedGraphicsColumn);
			}
		}

		public bool HasTextColumns
		{
			get { return TextColumns.IsNotNullOrEmpty(); }
		}

		List<TextColumnViewModel> _textColumns;
		public List<TextColumnViewModel> TextColumns
		{
			get { return _textColumns; }
			set
			{
				_textColumns = value;
				OnPropertyChanged(() => TextColumns);
			}
		}

		TextColumnViewModel _selectedTextColumn;
		public TextColumnViewModel SelectedTextColumn
		{
			get { return _selectedTextColumn; }
			set
			{
				_selectedTextColumn = value;
				OnPropertyChanged(() => SelectedTextColumn);
			}
		}
		#endregion

		#region Commands

		public RelayCommand SelectScheduleCommand { get; private set; }
		void OnSelectSchedule()
		{
			var scheduleSelectionViewModel = new ScheduleSelectionViewModel(Employee, SelectedSchedule, ScheduleStartDate);
			if (DialogService.ShowModalWindow(scheduleSelectionViewModel))
			{
				SelectedSchedule = scheduleSelectionViewModel.SelectedSchedule;
				if (SelectedSchedule != null)
				{
					ScheduleStartDate = scheduleSelectionViewModel.StartDate;
				}
			}
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentSelectionViewModel(Employee.OrganisationUID, SelectedDepartment != null ? SelectedDepartment.UID : Guid.Empty);
			departmentSelectionViewModel.Initialize();
			if (DialogService.ShowModalWindow(departmentSelectionViewModel))
			{
				SelectedDepartment = departmentSelectionViewModel.SelectedDepartment != null ? departmentSelectionViewModel.SelectedDepartment.Department : null;
			}
		}

		public RelayCommand SelectPositionCommand { get; private set; }
		void OnSelectPosition()
		{
			var positionSelectionViewModel = new PositionSelectionViewModel(Employee, SelectedPosition);
			if (DialogService.ShowModalWindow(positionSelectionViewModel))
			{
				SelectedPosition = positionSelectionViewModel.SelectedPosition;
			}
		}

		public RelayCommand SelectEscortCommand { get; private set; }
		void OnSelectEscort()
		{
			if (SelectedDepartment == null)
			{
				MessageBoxService.Show("Выберите подразделение");
				return;
			}
			var escortSelectionViewModel = new EscortSelectionViewModel(SelectedDepartment, SelectedEscort);
			if (DialogService.ShowModalWindow(escortSelectionViewModel))
			{
				SelectedEscort = escortSelectionViewModel.SelectedEmployee;
			}
		}

		#endregion

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		protected override bool Save()
		{
			var isLaunchEvent = IsLaunchEvent();
			Employee.FirstName = FirstName;
			Employee.SecondName = SecondName;
			Employee.LastName = LastName;
			Employee.DocumentNumber = DocumentNumber;
			Employee.BirthDate = BirthDate;
			Employee.BirthPlace = BirthPlace;
			Employee.DocumentGivenBy = GivenBy;
			Employee.DocumentGivenDate = GivenDate;
			Employee.Gender = Gender;
			Employee.DocumentValidTo = ValidTo;
			Employee.Citizenship = Citizenship;
			Employee.DocumentType = DocumentType;
			Employee.OrganisationUID = _organisation.UID;
			Employee.AdditionalColumns = (from x in TextColumns select x.AdditionalColumn).ToList();
			Employee.Phone = Phone;
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

			Employee.Department = SelectedDepartment;

			if (IsEmployee)
			{
				Employee.Position = SelectedPosition;
				Employee.Schedule = SelectedSchedule;
				Employee.ScheduleStartDate = ScheduleStartDate;
				Employee.CredentialsStartDate = CredentialsStartDate;
				Employee.TabelNo = TabelNo;
				if (IsOrganisationChief && _organisation.ChiefUID != Employee.UID)
					OrganisationHelper.SaveChief(_organisation.UID, Employee.UID, _organisation.Name);
				else if (_organisation.ChiefUID == Employee.UID && !IsOrganisationChief)
					OrganisationHelper.SaveChief(_organisation.UID, Guid.Empty, _organisation.Name);
				if (IsOrganisationHRChief && _organisation.HRChiefUID != Employee.UID)
					OrganisationHelper.SaveHRChief(_organisation.UID, Employee.UID, _organisation.Name);
				else if (_organisation.HRChiefUID == Employee.UID && !IsOrganisationHRChief)
					OrganisationHelper.SaveHRChief(_organisation.UID, Guid.Empty, _organisation.Name);
			}
			else
			{
				Employee.EscortUID = SelectedEscort != null ? SelectedEscort.UID : (Guid?)null;
				Employee.Description = Description;
			}
			Employee.Type = _personType;

			bool canSave;

			if (IsEmployee)
			{
				canSave = EmployeeHelper.Get(new EmployeeFilter {PersonType = PersonType.Employee}).Count(x => !x.IsDeleted) < 5;
			}
			else
			{
				canSave = EmployeeHelper.Get(new EmployeeFilter {PersonType = PersonType.Guest}).Count(x => !x.IsDeleted) < 2;
			}


			if (canSave)
			{
				var saveResult = EmployeeHelper.Save(Employee, _isNew);
				if (saveResult && isLaunchEvent)
					ServiceFactoryBase.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Publish(Employee);
				return saveResult;
			}
			else
			{
				MessageBoxService.ShowError(string.Format("Нельзя выполнить операцию из-за ограничения демо-версии. Максимальное количество {0} - {1}", IsEmployee ? "сотрудников" : "посетителей", IsEmployee ? 5 : 2));
				return false;
			}
		}

		bool IsLaunchEvent()
		{
			if ((Employee.Department != null && SelectedDepartment == null) ||
				(Employee.Position != null && SelectedPosition == null))
				return true;
			if((Employee.Department == null && SelectedDepartment != null) ||
				(Employee.Position == null && SelectedPosition != null))
				return true;
			return (SelectedDepartment != null && Employee.Department.UID != SelectedDepartment.UID) ||
			       (SelectedPosition != null && Employee.Position.UID != SelectedPosition.UID);
		}
	}
}