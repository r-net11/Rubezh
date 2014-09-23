using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortEmployee>
	{
		Organisation _organisation;
		PersonType _personType;

		public EmployeeDetailsViewModel() { }

		public bool Initialize(Organisation organisation, ShortEmployee employee, ViewPartViewModel parentViewModel)
		{
			var employeesViewModel = (parentViewModel as EmployeesViewModel);
			return Initialize(organisation.UID, employee, employeesViewModel.PersonType);
		}

		public bool Initialize(Guid organisationUID, ShortEmployee employee, PersonType personType, bool canEditDepartment = true, bool canEditPosition = true)
		{
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			SelectPositionCommand = new RelayCommand(OnSelectPosition);
			SelectScheduleCommand = new RelayCommand(OnSelectSchedule);
			SelectEscortCommand = new RelayCommand(OnSelectEscort);
			SelectBirthDateCommand = new RelayCommand(OnSelectBirthDate);
			SelectGivenDateCommand = new RelayCommand(OnSelectGivenDate);
			SelectValidToCommand = new RelayCommand(OnSelectValidTo);
			SelectGenderCommand = new RelayCommand(OnSelectGender);
			SelectDocumentTypeCommand = new RelayCommand(OnSelectDocumentType);
			SelectAppointedCommand = new RelayCommand(OnSelectAppointed);
			SelectCredentialsStartDateCommand = new RelayCommand(OnSelectCredentialsStartDate);

			CanEditDepartment = canEditDepartment;
			_canEditPosition = canEditPosition;

			_organisation = OrganisationHelper.GetSingle(organisationUID);
			_personType = personType;
			IsEmployee = _personType == PersonType.Employee;
			if (employee == null)
			{
				Employee = new Employee();
				Employee.OrganisationUID = organisationUID;
				Title = IsEmployee ? "Добавить сотрудника" : "Добавить посетителя";
				Employee.BirthDate = DateTime.Now;
				Employee.Appointed = DateTime.Now;
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
			EmployeeGuardZones = new EmployeeGuardZonesViewModel(Employee, _organisation);
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
			if (IsEmployee)
			{
				SelectedPosition = Employee.Position;
				SelectedSchedule = Employee.Schedule;
				ScheduleStartDate = Employee.ScheduleStartDate;
				Appointed = Employee.Appointed;
				CredentialsStartDate = Employee.CredentialsStartDate;
				TabelNo = Employee.TabelNo;
				IsOrganisationChief = _organisation.ChiefUID == Employee.UID;
			}
			else
			{
				SelectedEscort = EmployeeHelper.GetSingleShort(Employee.EscortUID);
			}
			SelectedDepartment = Employee.Department;
			TextColumns = new List<TextColumnViewModel>();
			GraphicsColumns = new List<IGraphicsColumnViewModel>();
			GraphicsColumns.Add(new PhotoColumnViewModel(Employee.Photo));
			SelectedGraphicsColumn = GraphicsColumns.FirstOrDefault();
			var additionalColumnTypes = AdditionalColumnTypeHelper.GetByOrganisation(Employee.OrganisationUID);
			if (additionalColumnTypes != null && additionalColumnTypes.Count() > 0)
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
					Cards = Employee.Cards,
					FirstName = FirstName,
					SecondName = SecondName,
					LastName = LastName,
					Type = Employee.Type,
					Appointed = Employee.Appointed.ToString("d MMM yyyy"),
					TextColumns = new List<TextColumn>()
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
		
		ShortDepartment selectedDepartment;
		public ShortDepartment SelectedDepartment
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
			get { return SelectedEscort != null; }
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
			get { return SelectedPosition != null; }
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

		public bool HasSelectedSchedule
		{
			get { return SelectedSchedule != null; }
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

		DateTime _appointed;
		public DateTime Appointed
		{
			get { return _appointed; }
			set
			{
				if (_appointed != value)
				{
					_appointed = value;
					OnPropertyChanged(() => Appointed);
					OnPropertyChanged(() => AppointedString);
				}
			}
		}
		public string AppointedString
		{
			get { return Appointed.ToString("dd/MM/yyyy"); }
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
					OnPropertyChanged(() => CredentialsStartDateString);
				}
			}
		}
		public string CredentialsStartDateString
		{
			get { return CredentialsStartDate.ToString("dd/MM/yyyy"); }
		}

		int _tabelNo;
		public int TabelNo
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

		public EmployeeGuardZonesViewModel EmployeeGuardZones { get; private set; }

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
				OnPropertyChanged(() => BirthDateString);
			}
		}

		public string BirthDateString
		{
			get { return BirthDate.ToString("dd/MM/yyyy"); }
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
				OnPropertyChanged(() => GivenDateString);
			}
		}

		public string GivenDateString
		{
			get { return GivenDate.ToString("dd/MM/yyyy"); }
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
				OnPropertyChanged(() => ValidToString);
			}
		}

		public string ValidToString
		{
			get { return ValidTo.ToString("dd/MM/yyyy"); }
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
		public RelayCommand SelectDocumentTypeCommand { get; private set; }
		void OnSelectDocumentType()
		{
			var selectDateViewModel = new DocumentTypeSelectionViewModel(DocumentType);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				DocumentType = selectDateViewModel.DocumentType;
			}
		}

		public RelayCommand SelectAppointedCommand { get; private set; }
		void OnSelectAppointed()
		{
			var selectDateViewModel = new DateSelectionViewModel(Appointed);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				Appointed = selectDateViewModel.DateTime;
			}
		}

		public RelayCommand SelectCredentialsStartDateCommand { get; private set; }
		void OnSelectCredentialsStartDate()
		{
			var selectDateViewModel = new DateSelectionViewModel(CredentialsStartDate);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				CredentialsStartDate = selectDateViewModel.DateTime;
			}
		}

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

		public RelayCommand SelectGenderCommand { get; private set; }
		void OnSelectGender()
		{
			var selectDateViewModel = new GenderSelectionViewModel(Gender);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				Gender = selectDateViewModel.Gender;
			}
		}

		public RelayCommand SelectGivenDateCommand { get; private set; }
		void OnSelectGivenDate()
		{
			var selectDateViewModel = new DateSelectionViewModel(GivenDate);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				GivenDate = selectDateViewModel.DateTime;
			}
		}

		public RelayCommand SelectValidToCommand { get; private set; }
		void OnSelectValidTo()
		{
			var selectDateViewModel = new DateSelectionViewModel(ValidTo);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				ValidTo = selectDateViewModel.DateTime;
			}
		}

		public RelayCommand SelectBirthDateCommand { get; private set; }
		void OnSelectBirthDate()
		{
			var selectDateViewModel = new DateSelectionViewModel(BirthDate);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				BirthDate = selectDateViewModel.DateTime;
			}
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentSelectionViewModel(Employee, SelectedDepartment);
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
				MessageBoxService.Show("Выберите отдел");
				return;
			}
			var escortSelectionViewModel = new EscortSelectionViewModel(SelectedDepartment, SelectedEscort);
			if (DialogService.ShowModalWindow(escortSelectionViewModel))
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
				Employee.Appointed = Appointed;
				Employee.CredentialsStartDate = CredentialsStartDate;
				Employee.TabelNo = TabelNo;
				if (IsOrganisationChief && _organisation.ChiefUID != Employee.UID)
					OrganisationHelper.SaveChief(_organisation.UID, Employee.UID);
				else if (_organisation.ChiefUID == Employee.UID && !IsOrganisationChief)
					OrganisationHelper.SaveChief(_organisation.UID, Guid.Empty);
			}
			else
			{
				Employee.EscortUID = SelectedEscort != null ? SelectedEscort.UID : (Guid?)null;
			}
			Employee.Type = _personType;

			var guardZoneAccesses = new List<XGuardZoneAccess>();
			foreach (var guardZone in EmployeeGuardZones.GuardZones)
			{
				if (guardZone.IsChecked)
				{
					var guardZoneAccess = new XGuardZoneAccess()
					{
						ZoneUID = guardZone.GuardZone.UID,
						CanSet = guardZone.CanSetZone,
						CanReset = guardZone.CanUnSetZone
					};
					guardZoneAccesses.Add(guardZoneAccess);
				}
			}
			Employee.GuardZoneAccesses = guardZoneAccesses;
			var saveResult = EmployeeHelper.Save(Employee);
			if (saveResult && isLaunchEvent)
				ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Publish(Employee);
			return saveResult;
		}

		bool IsLaunchEvent()
		{
			if((Employee.Department == null && SelectedDepartment != null) ||
				(Employee.Position == null && SelectedPosition != null))
				return true;
			if ((Employee.Department.UID != SelectedDepartment.UID) ||
				(Employee.Position.UID != SelectedPosition.UID))
				return true;
			return false;
		}
	}
}