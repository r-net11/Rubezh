using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		Organisation Organisation { get; set; }
		
		public EmployeeDetailsViewModel(PersonType personType, Organisation orgnaisation, ShortEmployee employee = null)
		{
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			RemoveDepartmentCommand = new RelayCommand(OnRemoveDepartment);
			SelectEscortCommand = new RelayCommand(OnSelectEscort);
			RemoveEscortCommand = new RelayCommand(OnRemoveEscort);
			SelectPositionCommand = new RelayCommand(OnSelectPosition);
			RemovePositionCommand = new RelayCommand(OnRemovePosition);
			SelectBirthDateCommand = new RelayCommand(OnSelectBirthDate);
			SelectGivenDateCommand = new RelayCommand(OnSelectGivenDate);
			SelectValidToCommand = new RelayCommand(OnSelectValidTo);
			SelectGenderCommand = new RelayCommand(OnSelectGender);
			SelectDocumentTypeCommand = new RelayCommand(OnSelectDocumentType);
			SelectAppointedCommand = new RelayCommand(OnSelectAppointed);
			SelectDismissedCommand = new RelayCommand(OnSelectDismissed);
			SelectCredentialsStartDateCommand = new RelayCommand(OnSelectCredentialsStartDate);
			SelectScheduleCommand = new RelayCommand(OnSelectSchedule);
			RemoveScheduleCommand = new RelayCommand(OnRemoveSchedule);

			Organisation = orgnaisation;
			IsEmployee = personType == PersonType.Employee;
			if (employee == null)
			{
				Employee = new Employee();
				Employee.OrganisationUID = orgnaisation.UID;
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
			EmployeeGuardZones = new EmployeeGuardZonesViewModel(Employee);
			CopyProperties();
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
				Dismissed = Employee.Dismissed;
				CredentialsStartDate = Employee.CredentialsStartDate;
			}
			else
			{
				var escortEmployee = EmployeeHelper.GetSingleShort(Employee.EscortUID);
				if(escortEmployee != null)
					SelectedEscort = new SelectationEmployeeViewModel(escortEmployee);
			}

			SelectedDepartment = Employee.Department;
			TextColumns = new List<TextColumnViewModel>();
			GraphicsColumns = new List<IGraphicsColumnViewModel>();
			GraphicsColumns.Add(new PhotoColumnViewModel(Employee.Photo));
			SelectedGraphicsColumn = GraphicsColumns.FirstOrDefault();
			var additionalColumnTypes = AdditionalColumnTypeHelper.GetByOrganisation(Employee.OrganisationUID);
			if (additionalColumnTypes != null && additionalColumnTypes.Count() > 0)
			{
				var graphicsColumnTypes = additionalColumnTypes.Where(x => x.DataType == AdditionalColumnDataType.Graphics);
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

		public Employee Employee { get; private set; }
		public ShortEmployee ShortEmployee
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
					Dismissed = Employee.Dismissed.ToString("d MMM yyyy"),
				};
				if (SelectedDepartment != null)
					result.DepartmentName = SelectedDepartment.Name;
				if (SelectedPosition != null)
					result.PositionName = SelectedPosition.Name;
				return result;
			}
		}
		public bool IsEmployee { get; private set; }

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

		SelectationEmployeeViewModel selectedEscort;
		public SelectationEmployeeViewModel SelectedEscort
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
				if(HasSelectedSchedule)
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

		DateTime _dismissed;
		public DateTime Dismissed
		{
			get { return _dismissed; }
			set
			{
				if (_dismissed != value)
				{
					_dismissed = value;
					OnPropertyChanged(() => Dismissed);
					OnPropertyChanged(() => DismissedString);
				}
			}
		}
		public string DismissedString
		{
			get { return Dismissed.ToString("dd/MM/yyyy"); }
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

		public RelayCommand SelectDismissedCommand { get; private set; }
		void OnSelectDismissed()
		{
			var selectDateViewModel = new DateSelectionViewModel(Dismissed);
			if (DialogService.ShowModalWindow(selectDateViewModel))
			{
				Dismissed = selectDateViewModel.DateTime;
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
			var selectScheduleViewModel = new SelectScheduleViewModel(Employee);
			if (DialogService.ShowModalWindow(selectScheduleViewModel))
			{
				var selectedSchedule = selectScheduleViewModel.SelectedSchedule;
				if(selectedSchedule != null)
				{
					SelectedSchedule = selectedSchedule.Schedule;
					ScheduleStartDate = selectScheduleViewModel.StartDate;
				}
			}
		}

		public RelayCommand RemoveScheduleCommand { get; private set; }
		void OnRemoveSchedule()
		{
			SelectedSchedule = null;
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
			var selectDepartmentViewModel = new SelectDepartmentViewModel(Employee);
			if (DialogService.ShowModalWindow(selectDepartmentViewModel))
			{
				SelectedDepartment = selectDepartmentViewModel.SelectedDepartment.Department;
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
			var selectPositionViewModel = new SelectPositionViewModel(Employee);
			if (DialogService.ShowModalWindow(selectPositionViewModel))
			{
				SelectedPosition = selectPositionViewModel.SelectedPosition.Position;
			}
		}

		public RelayCommand RemovePositionCommand { get; private set; }
		void OnRemovePosition()
		{
			SelectedPosition = null;
		}
		
		public RelayCommand SelectEscortCommand { get; private set; }
		void OnSelectEscort()
		{
			if(SelectedDepartment == null)
			{
				MessageBoxService.Show("Выберите отдел");
				return;
			}	
			Guid? escortUID = null; 
			if(HasSelectedEscort)
				escortUID = SelectedEscort.Employee.UID;
			var selectEscortViewModel = new SelectEscortViewModel(SelectedDepartment, escortUID);
			if (DialogService.ShowModalWindow(selectEscortViewModel))
			{
				SelectedEscort = selectEscortViewModel.SelectedEmployee;
			}
		}
		bool CanSelectEscort()
		{
			return SelectedDepartment != null;
		}

		public RelayCommand RemoveEscortCommand { get; private set; }
		void OnRemoveEscort()
		{
			SelectedEscort = null;
		}
		#endregion

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}
		
		protected override bool Save()
		{
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
			Employee.OrganisationUID = Organisation.UID;
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

			if (SelectedDepartment == null)
			{
				MessageBoxService.ShowWarning("Выберите отдел");
				return false;
			}
			Employee.Department = SelectedDepartment;

			if (IsEmployee)
			{

				if (SelectedPosition == null)
				{
					MessageBoxService.ShowWarning("Выберите должность");
					return false;
				}
				Employee.Position = SelectedPosition;
				if (SelectedSchedule == null)
				{
					MessageBoxService.ShowWarning("Выберите график работы");
					return false;
				}
				Employee.Schedule = SelectedSchedule;
				Employee.ScheduleStartDate = ScheduleStartDate;
			}
			else
			{
				Employee.EscortUID = SelectedEscort != null ? SelectedEscort.Employee.UID : Employee.EscortUID = null;
			}

			return EmployeeHelper.Save(Employee);
		}
	}
}