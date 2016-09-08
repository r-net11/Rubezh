using System.Linq;
using System.Threading.Tasks;
using FiresecClient;
using Localization.SKD.ViewModels;
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
				Title = CommonViewModels.CreateDepart;
				IsNew = true;
				var parentModel = (parentViewModel as DepartmentsViewModel).SelectedItem.Model;
				Department = new Department
				{
					Name = CommonViewModels.NewDepart,
					ParentDepartmentUID = parentModel != null ? parentModel.UID : Guid.Empty,
					OrganisationUID = OrganisationUID
				};
				_childDepartments = new Dictionary<Guid, string>();
			}
			else
			{
				Department = DepartmentHelper.GetDetails(shortDepartment.UID);
				Title = string.Format(CommonViewModels.DepartProperties, Department.Name);
				_childDepartments = new Dictionary<Guid, string>();
			}
			CopyProperties();
			ChiefViewModel = new EmployeeSelectationViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
			SelectDepartmentCommand = new RelayCommand(OnSelectDepartment);
			InitializeCommands();
			return true;
		}

		private void InitializeCommands()
		{
			SelectAccessTemplateCommand = new RelayCommand(OnSelectAccessTemplate);
			SelectScheduleCommand = new RelayCommand(OnSelectSchedule);
			SelectPassCardTemplateCommand = new RelayCommand(OnSelectPassCardTemplate);
		}

		public void Initialize(Guid organisationUID, Guid? parentDepartmentUID)
		{
			OrganisationUID = organisationUID;
			Title = CommonViewModels.CreateDepart;
			Department = new Department
			{
				Name = CommonViewModels.NewDepart,
				ParentDepartmentUID = parentDepartmentUID,
				OrganisationUID = OrganisationUID
			};
			CopyProperties();
			ChiefViewModel = new EmployeeSelectationViewModel(Department.ChiefUID, new EmployeeFilter { DepartmentUIDs = new List<Guid> { Department.UID } });
			InitializeCommands();
		}

		public void CopyProperties()
		{
			Name = Department.Name;
			Description = Department.Description;
			Phone = Department.Phone;
			SetSelectedDepartmentAsync();

			if (Department.Photo != null)
				PhotoData = Department.Photo.Data;

			SetSelectedAccessTemplateAsync();

			SetSelectedScheduleAsync();

			SetSelectedPassCardTemplateAsync();
		}

		private void SetSelectedDepartmentAsync()
		{
			CanSelectDepartment = false;
			Task.Factory.StartNew(() =>
			{
				return !Department.ParentDepartmentUID.IsNullOrEmpty()
					? DepartmentHelper.GetSingleShort(Department.ParentDepartmentUID.Value)
					: null;
			}).ContinueWith(t =>
			{
				CanSelectDepartment = true;
				if (t.IsCompleted)
					SelectedDepartment = t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedPassCardTemplateAsync()
		{
			CanSelectPassCardTemplate = false;
			Task.Factory.StartNew(() =>
			{
				return PassCardTemplateHelper.Get(new PassCardTemplateFilter
				{
					LogicalDeletationType = LogicalDeletationType.Active,
					OrganisationUIDs = new List<Guid> {OrganisationUID},
					UserUID = FiresecManager.CurrentUser.UID
				}).FirstOrDefault(x => x.UID == Department.PassCardTemplateUID);
			}).ContinueWith(t =>
			{
				CanSelectPassCardTemplate = true;
				if (t.IsCompleted)
					SelectedPassCardTemplate = t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedScheduleAsync()
		{
			CanSelectSchedule = false;
			Task.Factory.StartNew(() =>
			{
				return ScheduleHelper.Get(new ScheduleFilter
				{
					LogicalDeletationType = LogicalDeletationType.Active,
					OrganisationUIDs = new List<Guid> { OrganisationUID },
					UserUID = FiresecManager.CurrentUser.UID
				}).FirstOrDefault(x => x.UID == Department.ScheduleUID);
			}).ContinueWith(t =>
			{
				CanSelectSchedule = true;
				if (t.IsCompleted)
					SelectedSchedule = t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedAccessTemplateAsync()
		{
			CanSelectAccessTemplate = false;
			Task.Factory.StartNew(() =>
			{
				return AccessTemplateHelper.Get(new AccessTemplateFilter
				{
					LogicalDeletationType = LogicalDeletationType.Active,
					OrganisationUIDs = new List<Guid> {OrganisationUID},
					UserUID = FiresecManager.CurrentUser.UID
				}).FirstOrDefault(x => x.UID == Department.AccessTemplateUID);
			}).ContinueWith(t =>
			{
				CanSelectAccessTemplate = true;
				if (t.IsCompleted)
					SelectedAccessTemplate = t.Result;
			}, TaskScheduler.FromCurrentSynchronizationContext());
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
		/// <summary>
		/// Родительское подразделение
		/// </summary>
		public ShortDepartment SelectedDepartment
		{
			get { return _selectedDepartment; }
			private set
			{
				_selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
			}
		}

		private bool _canSelectDepartment;
		/// <summary>
		/// Определяет активность ссылки для выбора родительского подразделения
		/// </summary>
		public bool CanSelectDepartment
		{
			get { return _canSelectDepartment; }
			set
			{
				_canSelectDepartment = value;
				OnPropertyChanged(() => CanSelectDepartment);
		}
		}

		private AccessTemplate _selectedAccessTemplate;
		/// <summary>
		/// Шаблон доступа по умолчанию
		/// </summary>
		public AccessTemplate SelectedAccessTemplate
		{
			get { return _selectedAccessTemplate; }
			set
			{
				_selectedAccessTemplate = value;
				OnPropertyChanged(() => SelectedAccessTemplate);
			}
		}

		private bool _canSelectAccessTemplate;
		/// <summary>
		/// Определяет активность ссылки для выбора шаблона доступа по умолчанию
		/// </summary>
		public bool CanSelectAccessTemplate
		{
			get { return _canSelectAccessTemplate; }
			set
			{
				_canSelectAccessTemplate = value;
				OnPropertyChanged(() => CanSelectAccessTemplate);
			}
		}

		private Schedule _selectedSchedule;
		/// <summary>
		/// График работы по умолчанию
		/// </summary>
		public Schedule SelectedSchedule
		{
			get { return _selectedSchedule; }
			set
			{
				_selectedSchedule = value;
				OnPropertyChanged(() => SelectedSchedule);
			}
		}

		private bool _canSelectSchedule;
		/// <summary>
		/// Определяет активность ссылки для выбора графика работы по умолчанию
		/// </summary>
		public bool CanSelectSchedule
		{
			get { return _canSelectSchedule; }
			set
			{
				_canSelectSchedule = value;
				OnPropertyChanged(() => CanSelectSchedule);
			}
		}

		private ShortPassCardTemplate _selectedPassCardTemplate;
		/// <summary>
		/// Шаблон пропуска по умолчанию
		/// </summary>
		public ShortPassCardTemplate SelectedPassCardTemplate
		{
			get { return _selectedPassCardTemplate; }
			set
			{
				_selectedPassCardTemplate = value;
				OnPropertyChanged(() => SelectedPassCardTemplate);
			}
		}

		private bool _canSelectPassCardTemplate;
		/// <summary>
		/// Определяет активность ссылки для выбора шаблона пропуска по умолчанию
		/// </summary>
		public bool CanSelectPassCardTemplate
		{
			get { return _canSelectPassCardTemplate; }
			set
			{
				_canSelectPassCardTemplate = value;
				OnPropertyChanged(() => CanSelectPassCardTemplate);
			}
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
			var accessTemplateSelectionViewModel = new DepartmentAccessTemplateSelectionViewModel();
			accessTemplateSelectionViewModel.Initialize(OrganisationUID, selectedItem: SelectedAccessTemplate);
			if (DialogService.ShowModalWindow(accessTemplateSelectionViewModel))
			{
				SelectedAccessTemplate = accessTemplateSelectionViewModel.SelectedItem;
			}
		}

		public RelayCommand SelectScheduleCommand { get; private set; }
		private void OnSelectSchedule()
		{
			var scheduleSelectionViewModel = new DepartmentScheduleSelectionViewModel();
			scheduleSelectionViewModel.Initialize(OrganisationUID, selectedItem: SelectedSchedule);
			if (DialogService.ShowModalWindow(scheduleSelectionViewModel))
			{
				SelectedSchedule = scheduleSelectionViewModel.SelectedItem;
			}
		}

		public RelayCommand SelectPassCardTemplateCommand { get; private set; }
		private void OnSelectPassCardTemplate()
		{
			var passCardTemplateSelectionViewModel = new DepartmentPassCardTemplateSelectionViewModel();
			passCardTemplateSelectionViewModel.Initialize(OrganisationUID, selectedItem: SelectedPassCardTemplate);
			if (DialogService.ShowModalWindow(passCardTemplateSelectionViewModel))
			{
				SelectedPassCardTemplate = passCardTemplateSelectionViewModel.SelectedItem;
			}
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
			Department.AccessTemplateUID = SelectedAccessTemplate != null
				? SelectedAccessTemplate.UID
				: (Guid?) null;
			Department.ScheduleUID = SelectedSchedule != null
				? SelectedSchedule.UID
				: (Guid?) null;
			Department.PassCardTemplateUID = SelectedPassCardTemplate != null
				? SelectedPassCardTemplate.UID
				: (Guid?) null;

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