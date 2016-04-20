using System;
using System.Linq;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortDepartment>
	{
		Guid OrganisationUID { get; set; }
		Department Department { get; set; }
		public EmployeeSelectationViewModel ChiefViewModel { get; private set; }
		public bool IsNew { get; private set; }
		List<TinyDepartment> _childDepartments;
		/// <summary>
		/// Подразделения, принадлежащие выбранной организации
		/// </summary>
		IEnumerable<ShortDepartment> _orgianisationDepartments;

		public DepartmentDetailsViewModel() : base() { 	}

		/// <summary>
		/// Инициализатор для картотеки
		/// </summary>
		/// <param name="organisation"></param>
		/// <param name="shortDepartment"></param>
		/// <param name="parentViewModel"></param>
		/// <returns></returns>
		public bool Initialize(Organisation organisation, ShortDepartment shortDepartment, ViewPartViewModel parentViewModel)
		{
			var departmentsViewModel = parentViewModel as DepartmentsViewModel;
			InitializeInternal(organisation.UID, shortDepartment, departmentsViewModel.SelectedItem.Model, departmentsViewModel.Models);
			return true;
		}

		/// <summary>
		/// Инициализатор для создания нового подразделения вне картотеки
		/// </summary>
		/// <param name="organisationUID"></param>
		/// <param name="parentDepartment"></param>
		public void Initialize(Guid organisationUID, ShortDepartment parentDepartment, IEnumerable<ShortDepartment> organisationDepartments)
		{
			InitializeInternal(organisationUID, null, parentDepartment, organisationDepartments);
		}

		void InitializeInternal(Guid organisationUID, ShortDepartment department, ShortDepartment parentDepartment, IEnumerable<ShortDepartment> organisationDepartments)
		{
			OrganisationUID = organisationUID;
			if (department == null)
			{
				Title = "Создание подразделения";
				IsNew = true;
				var parentModel = parentDepartment;
				Department = new Department()
				{
					Name = "Новое подразделение",
					ParentDepartmentUID = parentModel != null ? parentModel.UID : Guid.Empty,
					OrganisationUID = OrganisationUID
				};
				_childDepartments = new List<TinyDepartment>();
			}
			else
			{
				Department = DepartmentHelper.GetDetails(department.UID);
				Title = string.Format("Свойства подразделения: {0}", Department.Name);
				
			}
			CopyProperties();
			ChiefViewModel = new EmployeeSelectationViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			_childDepartments = new List<TinyDepartment>();
			_orgianisationDepartments = organisationDepartments != null ? organisationDepartments : new List<ShortDepartment>();
		}

		public void CopyProperties()
		{
			Name = Department.Name;
			Description = Department.Description;
			Phone = Department.Phone;
			SelectedDepartment = DepartmentHelper.GetSingleShort(Department.ParentDepartmentUID);
			if (Department.Photo != null)
				PhotoData = Department.Photo.Data;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(() => Name);
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

		byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(() => PhotoData);
			}
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

		protected override bool CanSave()
		{
			return true;
		}

		public ShortDepartment Model
		{
			get
			{
				return new ShortDepartment
				{
					UID = Department.UID,
					Description = Department.Description,
					Name = Department.Name,
					ParentDepartmentUID = Department.ParentDepartmentUID,
					ChildDepartments = _childDepartments,
					Phone = Department.Phone,
					OrganisationUID = OrganisationUID,
					ChiefUID = ChiefViewModel.SelectedEmployeeUID
				};
			}
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentParentSelectionViewModel(OrganisationUID, SelectedDepartment != null ? SelectedDepartment.UID : Guid.Empty, Department.UID);
			departmentSelectionViewModel.Initialize();
			if (DialogService.ShowModalWindow(departmentSelectionViewModel))
			{
				SelectedDepartment = departmentSelectionViewModel.SelectedDepartment != null ? departmentSelectionViewModel.SelectedDepartment.Department : null;
			}
		}

		public bool ValidateAndSave()
		{
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			if(_orgianisationDepartments.Any(x => 
				x.UID != Department.UID &&
				!x.IsDeleted &&
				x.ParentDepartmentUID == Department.ParentDepartmentUID && 
				x.OrganisationUID == Department.OrganisationUID && 
				x.Name == Department.Name))
			{
				ServiceFactory.MessageBoxService.Show("Невозможно добавить подразделение с совпадающим именем");
				return false;
			}
			return DepartmentHelper.Save(Department, IsNew);
		}

		protected override bool Save()
		{
			Department.Name = Name;
			Department.Description = Description;
			if (Department.Photo == null)
				Department.Photo = new Photo();
			Department.Photo.Data = PhotoData;
			Department.ChiefUID = ChiefViewModel.SelectedEmployeeUID;
			Department.ParentDepartmentUID = SelectedDepartment != null ? SelectedDepartment.UID : Guid.Empty;
			Department.Phone = Phone;
			var saveResult = ValidateAndSave();
			if (saveResult)
			{
				ServiceFactory.Events.GetEvent<ChangeDepartmentChiefEvent>().Publish(Department);
				return true;
			}
			else
				return false;
		}

		bool Validate()
		{
			if (Department.Phone.Length > 50)
			{
				MessageBoxService.Show("Значение поля 'Телефон' не может быть длиннее 50 символов");
				return false;
			}
			return true;
		}
	}
}