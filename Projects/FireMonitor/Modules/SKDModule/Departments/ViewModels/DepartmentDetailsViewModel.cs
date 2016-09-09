using System.Threading;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.ViewModels;
using SKDModule.Events;
using StrazhAPI.Extensions;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKDModule.ViewModels
{
	public class DepartmentDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<ShortDepartment>
	{
		private string _name;
		private string _description;
		private string _phone;
		private byte[] _photoData;
		private ShortDepartment _selectedDepartment;
		private AccessTemplate _selectedAccessTemplate;
		private Schedule _selectedSchedule;
		private ShortPassCardTemplate _selectedPassCardTemplate;
		private bool _canSelectDepartment;
		private bool _canSelectAccessTemplate;
		private bool _canSelectSchedule;
		private bool _canSelectPassCardTemplate;
		private Dictionary<Guid, string> _childDepartments;

		#region Properties
		private Organisation CurrentOrganisation { get; set; }
		private Department Department { get; set; }
		public EmployeeSelectationViewModel ChiefViewModel { get; private set; }
		public bool IsNew { get; private set; }
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

		public byte[] PhotoData
		{
			get { return _photoData; }
			set
			{
				_photoData = value;
				OnPropertyChanged(() => PhotoData);
			}
		}

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
					//OrganisationUID = OrganisationUID
					OrganisationUID = CurrentOrganisation.UID
				};
			}
		}
		#endregion

		public bool Initialize(Organisation organisation, ShortDepartment shortDepartment, ViewPartViewModel parentViewModel)
		{
			CurrentOrganisation = organisation;
			if (shortDepartment == null)
			{
				Title = CommonViewModels.CreateDepart;
				IsNew = true;

				var departmentsViewModel = parentViewModel as DepartmentsViewModel;

				if (departmentsViewModel != null)
				{
					var parentModel = departmentsViewModel.SelectedItem.Model;
					Department = new Department
					{
						Name = CommonViewModels.NewDepart,
						ParentDepartmentUID = parentModel != null ? parentModel.UID : Guid.Empty,
						OrganisationUID = CurrentOrganisation.UID
					};
				}

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

		public void Initialize(Organisation organisation, Guid? parentDepartmentUID)
		{
			if(organisation == null)
				throw new ArgumentNullException("organisation");

			CurrentOrganisation = organisation;
			Title = CommonViewModels.CreateDepart;
			Department = new Department
			{
				Name = CommonViewModels.NewDepart,
				ParentDepartmentUID = parentDepartmentUID,
				OrganisationUID = CurrentOrganisation.UID
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

			Func<ShortDepartment> departmentFunc = () => Department.ParentDepartmentUID.IsNullOrEmpty()
				? null
				: DepartmentHelper.GetSingleShort(Department.ParentDepartmentUID.Value);

			Task.Factory.StartNew(departmentFunc)
				.ContinueWith(t =>
				{
					CanSelectDepartment = true;
					SelectedDepartment = t.Result;
				}, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedPassCardTemplateAsync()
		{
			if(CurrentOrganisation == null)
				throw new InvalidOperationException("Current organisation is null");

			CanSelectPassCardTemplate = false;

			var filter = new PassCardTemplateFilter
			{
				LogicalDeletationType = LogicalDeletationType.Active,
				OrganisationUIDs = new List<Guid> {CurrentOrganisation.UID},
				UserUID = FiresecManager.CurrentUser.UID
			};

			Task.Factory.StartNew(() => PassCardTemplateHelper.Get(filter))
			.ContinueWith(t =>
			{
				CanSelectPassCardTemplate = true;
				SelectedPassCardTemplate = t.Result.FirstOrDefault(x => x.UID == Department.PassCardTemplateUID);
			}, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedScheduleAsync()
		{
			if(CurrentOrganisation == null)
				throw new InvalidOperationException("Current organisation is null");

			CanSelectSchedule = false;

			var filter = new ScheduleFilter
			{
				LogicalDeletationType = LogicalDeletationType.Active,
				OrganisationUIDs = new List<Guid> {CurrentOrganisation.UID},
				UserUID = FiresecManager.CurrentUser.UID
			};

			Task.Factory.StartNew(() => ScheduleHelper.Get(filter))
			.ContinueWith(t =>
			{
				CanSelectSchedule = true;
				SelectedSchedule = t.Result.FirstOrDefault(x => x.UID == Department.ScheduleUID);
			}, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void SetSelectedAccessTemplateAsync()
		{
			if(CurrentOrganisation == null)
				throw new InvalidOperationException("Current organisation is null");

			CanSelectAccessTemplate = false;

			var filter = new AccessTemplateFilter
			{
				LogicalDeletationType = LogicalDeletationType.Active,
				OrganisationUIDs = new List<Guid> {CurrentOrganisation.UID},
				UserUID = FiresecManager.CurrentUser.UID
			};

			Task.Factory.StartNew(() => AccessTemplateHelper.Get(filter))
			.ContinueWith(t =>
			{
				CanSelectAccessTemplate = true;
				SelectedAccessTemplate = t.Result.FirstOrDefault(x => x.UID == Department.AccessTemplateUID);
			}, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void OnSelectDepartment()
		{
			var departmentSelectionViewModel = new DepartmentParentSelectionViewModel(CurrentOrganisation, SelectedDepartment != null
				? SelectedDepartment.UID
				: Guid.Empty, Department.UID);
			departmentSelectionViewModel.Initialize();

			if (DialogService.ShowModalWindow(departmentSelectionViewModel))
			{
				SelectedDepartment = departmentSelectionViewModel.SelectedDepartment != null ? departmentSelectionViewModel.SelectedDepartment.Department : null;
			}
		}

		private void OnSelectAccessTemplate()
		{
			var accessTemplateSelectionViewModel = new DepartmentAccessTemplateSelectionViewModel();
			accessTemplateSelectionViewModel.Initialize(CurrentOrganisation, selectedItem: SelectedAccessTemplate);

			if (DialogService.ShowModalWindow(accessTemplateSelectionViewModel))
			{
				SelectedAccessTemplate = accessTemplateSelectionViewModel.SelectedItem;
			}
		}

		private void OnSelectSchedule()
		{
			var scheduleSelectionViewModel = new DepartmentScheduleSelectionViewModel();
			scheduleSelectionViewModel.Initialize(CurrentOrganisation, selectedItem: SelectedSchedule);
			if (DialogService.ShowModalWindow(scheduleSelectionViewModel))
			{
				SelectedSchedule = scheduleSelectionViewModel.SelectedItem;
			}
		}

		private void OnSelectPassCardTemplate()
		{
			var passCardTemplateSelectionViewModel = new PassCardTemplateSelectionViewModel();
			passCardTemplateSelectionViewModel.Initialize(CurrentOrganisation, selectedItem: SelectedPassCardTemplate);
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

		public RelayCommand SelectDepartmentCommand { get; private set; }
		public RelayCommand SelectPassCardTemplateCommand { get; private set; }
		public RelayCommand SelectScheduleCommand { get; private set; }
		public RelayCommand SelectAccessTemplateCommand { get; private set; }
	}
}