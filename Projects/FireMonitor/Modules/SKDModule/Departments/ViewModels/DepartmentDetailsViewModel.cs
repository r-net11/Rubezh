using System.Diagnostics;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using System;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortDepartment>
	{
		private Guid OrganisationUID { get; set; }
		private Department Department { get; set; }
		public EmployeeSelectationViewModel ChiefViewModel { get; private set; }
		public bool IsNew { get; private set; }
		private Dictionary<Guid, string> _childDepartments;

		public bool Initialize(Organisation organisation, ShortDepartment shortDepartment, ViewPartViewModel parentViewModel)
		{
			OrganisationUID = organisation.UID;
			if (shortDepartment == null)
			{
				Title = "Создание подразделения";
				IsNew = true;
				var parentModel = (parentViewModel as DepartmentsViewModel).SelectedItem.Model;
				Department = new Department
				{
					Name = "Новое подразделение",
					ParentDepartmentUID = parentModel != null ? parentModel.UID : Guid.Empty,
					OrganisationUID = OrganisationUID
				};
				_childDepartments = new Dictionary<Guid, string>();
			}
			else
			{
				Department = DepartmentHelper.GetDetails(shortDepartment.UID);
				Title = string.Format("Свойства подразделения: {0}", Department.Name);
				_childDepartments = new Dictionary<Guid, string>();
			}
			CopyProperties();
			ChiefViewModel = new EmployeeSelectationViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			SelectAccessTemplateCommand = new RelayCommand(OnSelectAccessTemplate);
			SelectScheduleCommand = new RelayCommand(OnSelectSchedule);
			SelectPassCardTemplateCommand = new RelayCommand(OnSelectPassCardTemplate);
			return true;
		}

		public void Initialize(Guid organisationUID, Guid? parentDepartmentUID)
		{
			OrganisationUID = organisationUID;
			Title = "Создание подразделения";
			Department = new Department
			{
				Name = "Новое подразделение",
				ParentDepartmentUID = parentDepartmentUID,
				OrganisationUID = OrganisationUID
			};
			CopyProperties();
			ChiefViewModel = new EmployeeSelectationViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
		}

		public void CopyProperties()
		{
			Name = Department.Name;
			Description = Department.Description;
			Phone = Department.Phone;
			SelectedDepartment = !Department.ParentDepartmentUID.IsNullOrEmpty()
				? DepartmentHelper.GetSingleShort(Department.ParentDepartmentUID.Value)
				: null;

			if (Department.Photo != null)
				PhotoData = Department.Photo.Data;
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name == value) return;
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
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

		private string _phone;
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

		private byte[] _photoData;
		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(()=>PhotoData);
			}
		}

		private ShortDepartment _selectedDepartment;
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
			get { return SelectedDepartment != null; }
		}

		private AccessTemplate _selectedAccessTemplate;
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged(() => SelectedAccessTemplate);
				OnPropertyChanged(() => HasSelectedAccessTemplate);
			}
		}

		public bool HasSelectedAccessTemplate
		{
			get { return SelectedAccessTemplate != null; }
		}

		private Schedule _selectedSchedule;
		public Schedule SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
				OnPropertyChanged(() => HasSelectedSchedule);
			}
		}

		public bool HasSelectedSchedule
		{
			get { return SelectedSchedule != null; }
		}

		private PassCardTemplate _selectedPassCardTemplate;
		public PassCardTemplate SelectedPassCardTemplate
		{
			get { return _selectedPassCardTemplate; }
			set
			{
				_selectedPassCardTemplate = value;
				OnPropertyChanged(() => SelectedPassCardTemplate);
				OnPropertyChanged(() => HasSelectedPassCardTemplate);
			}
		}

		public bool HasSelectedPassCardTemplate
		{
			get { return SelectedPassCardTemplate != null; }
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
					OrganisationUID = OrganisationUID
				};
			}
		}

		public RelayCommand SelectDepartmentCommand { get; private set; }
		private void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentParentSelectionViewModel(OrganisationUID, SelectedDepartment != null ? SelectedDepartment.UID : Guid.Empty, Department.UID);
			departmentSelectionViewModel.Initialize();
			if (DialogService.ShowModalWindow(departmentSelectionViewModel))
			{
				SelectedDepartment = departmentSelectionViewModel.SelectedDepartment != null ? departmentSelectionViewModel.SelectedDepartment.Department : null;
			}
		}

		public RelayCommand SelectAccessTemplateCommand { get; private set; }
		private void OnSelectAccessTemplate()
		{
		}

		public RelayCommand SelectScheduleCommand { get; private set; }
		private void OnSelectSchedule()
		{
		}

		public RelayCommand SelectPassCardTemplateCommand { get; private set; }
		private void OnSelectPassCardTemplate()
		{
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

			var saveResult = DepartmentHelper.Save(Department, IsNew);
			if (saveResult)
			{
				ServiceFactoryBase.Events.GetEvent<ChangeDepartmentChiefEvent>().Publish(Department);
				return true;
			}
			return false;
		}
	}
}