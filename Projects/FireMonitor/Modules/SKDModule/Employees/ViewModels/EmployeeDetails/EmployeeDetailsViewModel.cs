using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;

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

		public EmployeeDetailsViewModel() { }

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
				Genders.Add(item);
			
			DocumentTypes = new ObservableCollection<EmployeeDocumentType>();
			foreach (EmployeeDocumentType item in Enum.GetValues(typeof(EmployeeDocumentType)))
				DocumentTypes.Add(item);
			
			_organisation = OrganisationHelper.GetSingle(organisationUID);
			_personType = personType;
			IsEmployee = _personType == PersonType.Employee;
			_isNew = employee == null;
			if (_isNew)
			{
				Employee = new Employee();
				Employee.OrganisationUID = organisationUID;
				Title = IsEmployee ? "Добавить сотрудника" : "Добавить посетителя";
				Employee.BirthDate = DateTime.Now;
				Employee.CredentialsStartDate = DateTime.Now;
				Employee.DocumentGivenDate = DateTime.Now;
				Employee.DocumentValidTo = DateTime.Now;
				Employee.RemovalDate = DateTime.Now;
				Employee.ScheduleStartDate = DateTime.Now;
			}
			else
			{
				Employee = EmployeeHelper.GetDetails(employee.UID);
				if (Employee == null)
					Employee = new Employee();
				if (IsEmployee)
					Title = string.Format("Свойства сотрудника: {0}", employee.FIO);
				else
					Title = string.Format("Свойства посетителя: {0}", employee.FIO);
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
			BirthDate = Employee.BirthDate;
			BirthPlace = Employee.BirthPlace;
			GivenBy = Employee.DocumentGivenBy;
			GivenDate = Employee.DocumentGivenDate;
			Gender = Employee.Gender;
			ValidTo = Employee.DocumentValidTo;
			Citizenship = Employee.Citizenship;
			DocumentType = Employee.DocumentType;
			Phone = Employee.Phone;
			if (IsEmployee)
			{
				SelectedPosition = new EmployeeItem { Name = Employee.PositionName, UID = Employee.PositionUID, IsDeleted = Employee.IsPositionDeleted };
				SelectedSchedule = new EmployeeItem { Name = Employee.ScheduleName, UID = Employee.ScheduleUID, IsDeleted = Employee.IsScheduleDeleted };
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
			SelectedDepartment = new EmployeeItem { Name = Employee.DepartmentName, UID = Employee.DepartmentUID, IsDeleted = Employee.IsDepartmentDeleted };
			TextColumns = new List<TextColumnViewModel>();
			GraphicsColumns = new List<IGraphicsColumnViewModel>();
			GraphicsColumns.Add(new PhotoColumnViewModel(Employee.Photo));
			SelectedGraphicsColumn = GraphicsColumns.FirstOrDefault();
			var additionalColumnTypes = AdditionalColumnTypeHelper.GetByOrganisation(Employee.OrganisationUID);
			if (additionalColumnTypes != null && additionalColumnTypes.Count() > 0)
			{
				foreach (var columnType in additionalColumnTypes)
				{
					var columnValue = Employee.AdditionalColumns.FirstOrDefault(x => x.AdditionalColumnTypeUID == columnType.UID);
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
					CredentialsStartDate = CredentialsStartDate.ToString(),
					TextColumns = new List<TextColumn>(),
					Phone = Employee.Phone,
					Description = Employee.Description,
					OrganisationName = _organisation.Name,
					OrganisationUID = _organisation.UID
				};
				if (SelectedDepartment != null)
				{
					result.DepartmentName = SelectedDepartment.Name;
					result.IsDepartmentDeleted = SelectedDepartment.IsDeleted;
				}
				if (SelectedPosition != null)
				{
					result.PositionName = SelectedPosition.Name;
					result.IsPositionDeleted = SelectedPosition.IsDeleted;
				}
				foreach (var item in Employee.AdditionalColumns.Where(x => x.DataType == AdditionalColumnDataType.Text))
				{
					result.TextColumns.Add(new TextColumn { ColumnTypeUID = item.AdditionalColumnTypeUID, Text = item.TextData });
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

		EmployeeItem selectedDepartment;
		public EmployeeItem SelectedDepartment
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
			get { return EmployeeItem.IsNotNullOrEmpty(SelectedDepartment); }
		}

		ShortEmployee selectedEscort;
		public ShortEmployee SelectedEscort
		{
			get { return selectedEscort; }
			private set 
			{
				selectedEscort = value;
				OnPropertyChanged(() => SelectedEscort);
				OnPropertyChanged(() => HasSelectedEscort);
			}
		}

		public bool HasSelectedEscort
		{
			get { return SelectedEscort != null && !SelectedEscort.IsDeleted; }
		}

		EmployeeItem _selectedPosition;
		public EmployeeItem SelectedPosition
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
			get { return EmployeeItem.IsNotNullOrEmpty(SelectedPosition); }
		}

		EmployeeItem _selectedSchedule;
		public EmployeeItem SelectedSchedule
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
			get { return EmployeeItem.IsNotNullOrEmpty(SelectedSchedule); }
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
			get
			{
				if (HasSelectedSchedule)
					return string.Format("{0} с {1}", SelectedSchedule.Name, ScheduleStartDate.ToString("dd/MM/yyyy"));
				else
					return "";
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

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged(() => Description);
				}
			}
		}

		string _phone;
		public string Phone
		{
			get { return _phone; }
			set
			{
				if (_phone != value)
				{
					_phone = value;
					OnPropertyChanged(() => Phone);
				}
			}
		}

		DateTime _credentialsStartDate;
		public DateTime CredentialsStartDate
		{
			get { return _credentialsStartDate; }
			set
			{
				if (_credentialsStartDate != value)
				{
					_credentialsStartDate = value;
					OnPropertyChanged(() => CredentialsStartDate);
				}
			}
		}
		string _tabelNo;
		public string TabelNo
		{
			get { return _tabelNo; }
			set
			{
				if (_tabelNo != value)
				{
					_tabelNo = value;
					OnPropertyChanged(() => TabelNo);
				}
			}
		}

		bool _isOrganisationChief;
		public bool IsOrganisationChief
		{
			get { return _isOrganisationChief; }
			set
			{
				if (_isOrganisationChief != value)
				{
					_isOrganisationChief = value;
					OnPropertyChanged(() => IsOrganisationChief);
				}
			}
		}

		bool _isOrganisationHRChief;
		public bool IsOrganisationHRChief
		{
			get { return _isOrganisationHRChief; }
			set
			{
				if (_isOrganisationHRChief != value)
				{
					_isOrganisationHRChief = value;
					OnPropertyChanged(() => IsOrganisationHRChief);
				}
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

		DateTime _birthDate;
		public DateTime BirthDate
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

		DateTime _givenDate;
		public DateTime GivenDate
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

		Gender _gender;
		public Gender Gender
		{
			get { return _gender; }
			set
			{
				_gender = value;
				OnPropertyChanged(() => Gender);
				OnPropertyChanged(() => GenderString);
			}
		}

		public string GenderString
		{
			get { return Gender.ToDescription(); }
		}

		DateTime _validTo;
		public DateTime ValidTo
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

		EmployeeDocumentType _documentType;
		public EmployeeDocumentType DocumentType
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
		#endregion

		#region Commands
		public RelayCommand SelectScheduleCommand { get; private set; }
		void OnSelectSchedule()
		{
			var scheduleSelectionViewModel = new ScheduleSelectionViewModel(Employee, SelectedSchedule, ScheduleStartDate);
			if (ServiceFactory.DialogService.ShowModalWindow(scheduleSelectionViewModel))
			{
				if (scheduleSelectionViewModel.SelectedSchedule != null)
				{
					SelectedSchedule = new EmployeeItem
					{
						Name = scheduleSelectionViewModel.SelectedSchedule.Name,
						UID = scheduleSelectionViewModel.SelectedSchedule.UID,
						IsDeleted = scheduleSelectionViewModel.SelectedSchedule.IsDeleted
					};
					ScheduleStartDate = scheduleSelectionViewModel.StartDate;
				}
				else
					SelectedSchedule = null;
				
			}
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentSelectionViewModel(Employee.OrganisationUID, 
				SelectedDepartment != null ? SelectedDepartment.UID : Guid.Empty);
			departmentSelectionViewModel.Initialize();
			if (ServiceFactory.DialogService.ShowModalWindow(departmentSelectionViewModel))
			{
				if(departmentSelectionViewModel.SelectedDepartment != null)
					SelectedDepartment = new EmployeeItem 
					{ 
						Name = departmentSelectionViewModel.SelectedDepartment.Department.Name, 
						UID = departmentSelectionViewModel.SelectedDepartment.Department.UID, 
						IsDeleted = departmentSelectionViewModel.SelectedDepartment.Department.IsDeleted 
					};
				else
					SelectedDepartment = null; 
			}
		}

		public RelayCommand SelectPositionCommand { get; private set; }
		void OnSelectPosition()
		{
			var positionSelectionViewModel = new PositionSelectionViewModel(Employee, SelectedPosition);
			if (ServiceFactory.DialogService.ShowModalWindow(positionSelectionViewModel))
			{
				if (positionSelectionViewModel.SelectedPosition != null)
					SelectedPosition = new EmployeeItem
					{
						Name = positionSelectionViewModel.SelectedPosition.Name,
						UID = positionSelectionViewModel.SelectedPosition.UID,
						IsDeleted = positionSelectionViewModel.SelectedPosition.IsDeleted
					};
				else
					SelectedPosition = null; 
			}
		}

		public RelayCommand SelectEscortCommand { get; private set; }
		void OnSelectEscort()
		{
			if (SelectedDepartment == null)
			{
				ServiceFactory.MessageBoxService.Show("Выберите подразделение");
				return;
			}
			var escortSelectionViewModel = new EscortSelectionViewModel(SelectedDepartment, SelectedEscort, _organisation.UID);
			if (ServiceFactory.DialogService.ShowModalWindow(escortSelectionViewModel))
			{
				SelectedEscort = escortSelectionViewModel.SelectedEmployee;
			}
		}
		bool CanSelectEscort()
		{
			return SelectedDepartment != null;
		}
		#endregion

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		protected override bool Save()
		{
			bool isLaunchEvent = IsLaunchEvent();
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

			if (SelectedDepartment != null)
			{
				Employee.DepartmentUID = SelectedDepartment.UID;
				Employee.DepartmentName = SelectedDepartment.Name;
				Employee.IsDepartmentDeleted = SelectedDepartment.IsDeleted;
			}
			else
				Employee.DepartmentUID = Guid.Empty;

			if (IsEmployee)
			{
				if (SelectedPosition != null)
				{
					Employee.PositionUID = SelectedPosition.UID;
					Employee.PositionName = SelectedPosition.Name;
					Employee.IsPositionDeleted = SelectedPosition.IsDeleted;
				}
				else
					Employee.PositionUID = Guid.Empty;

				if (SelectedSchedule != null)
				{
					Employee.ScheduleUID = SelectedSchedule.UID;
					Employee.ScheduleName = SelectedSchedule.Name;
					Employee.IsScheduleDeleted = SelectedSchedule.IsDeleted;
					Employee.ScheduleStartDate = ScheduleStartDate;
				}
				else
					Employee.ScheduleUID = Guid.Empty;
				Employee.CredentialsStartDate = CredentialsStartDate;
				Employee.TabelNo = TabelNo;
				
			}
			else
			{
				Employee.EscortUID = SelectedEscort != null ? SelectedEscort.UID : (Guid?)null;
				Employee.Description = Description;
			}
			Employee.Type = _personType;

			if (!Validate())
				return false;
			var saveResult = EmployeeHelper.Save(Employee, _isNew);
			if (saveResult)
			{
				bool isChiefChanged = false;
				if (IsOrganisationChief && _organisation.ChiefUID != Employee.UID)
					isChiefChanged = OrganisationHelper.SaveChief(_organisation.UID, Employee.UID, _organisation.Name);
				else if (_organisation.ChiefUID == Employee.UID && !IsOrganisationChief)
					isChiefChanged = OrganisationHelper.SaveChief(_organisation.UID, null, _organisation.Name);
				if (IsOrganisationHRChief && _organisation.HRChiefUID != Employee.UID)
					isChiefChanged = OrganisationHelper.SaveHRChief(_organisation.UID, Employee.UID, _organisation.Name);
				else if (_organisation.HRChiefUID == Employee.UID && !IsOrganisationHRChief)
					isChiefChanged = OrganisationHelper.SaveHRChief(_organisation.UID, null, _organisation.Name);
				if (isChiefChanged)
					ServiceFactory.Events.GetEvent<ChiefChangedEvent>().Publish(null);
				if (isLaunchEvent)
					ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Publish(Employee);
			}
			return saveResult;
		}

		bool IsLaunchEvent()
		{
			if ((Employee.DepartmentUID != Guid.Empty && SelectedDepartment == null) ||
				(Employee.PositionUID != Guid.Empty && SelectedPosition == null))
				return true;
			if ((Employee.DepartmentUID == Guid.Empty && SelectedDepartment != null) ||
				(Employee.PositionUID == Guid.Empty && SelectedPosition != null))
				return true;
			if ((SelectedDepartment != null && Employee.DepartmentUID != SelectedDepartment.UID) ||
				(SelectedPosition != null && Employee.PositionUID != SelectedPosition.UID))
				return true;
			return false;
		}

		bool Validate()
		{
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			if(Employee.FirstName.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Имя' не может быть длиннее 50 символов");
				return false;
			}
			if (Employee.TabelNo != null && Employee.TabelNo.Length > 40)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Табельный номер' не может быть длиннее 40 символов");
				return false;
			}
			if (Employee.SecondName != null && Employee.SecondName.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Отчество' не может быть длиннее 50 символов");
				return false;
			}
			if (Employee.LastName != null && Employee.LastName.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Фамилия' не может быть длиннее 50 символов");
				return false;
			}
			if (Employee.DocumentNumber != null && Employee.DocumentNumber.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Номер документа' не может быть длиннее 50 символов");
				return false;
			}
			if (Employee.Phone != null && Employee.Phone.Length > 50)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Телефон' не может быть длиннее 50 символов");
				return false;
			}
			if (Employee.DocumentGivenBy != null && Employee.DocumentGivenBy.Length > 4000)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Документ выдан' не может быть длиннее 4000 символов");
				return false;
			}
			if (Employee.BirthPlace != null && Employee.BirthPlace.Length > 4000)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Место рождения' не может быть длиннее 4000 символов");
				return false;
			}
			if (Employee.Citizenship != null && Employee.Citizenship.Length > 4000)
			{
				ServiceFactory.MessageBoxService.Show("Значение поля 'Гражданство' не может быть длиннее 4000 символов");
				return false;
			}
			return true;
		}	
	}

	public class EmployeeItem
	{
		public Guid UID { get; set; }
		public string Name { get; set; }
		public bool IsDeleted { get; set; }

		public static bool IsNotNullOrEmpty(EmployeeItem employeeItem)
		{
			return employeeItem != null && employeeItem.UID != Guid.Empty && !employeeItem.IsDeleted;
		}
	}

}