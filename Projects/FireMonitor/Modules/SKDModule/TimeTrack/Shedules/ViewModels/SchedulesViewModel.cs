using Localization.SKD.Common;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class SchedulesViewModel : OrganisationBaseViewModel<Schedule, ScheduleFilter, ScheduleViewModel, ScheduleDetailsViewModel>, ISelectable<Guid>, ITimeTrackItemsViewModel, ICardDoorsParentList<ScheduleViewModel>
	{
		bool _isInitialized;
		ChangeIsDeletedSubscriber _changeIsDeletedSubscriber;
		public LogicalDeletationType LogicalDeletationType { get; set; }

		public SchedulesViewModel()
		{
			_isInitialized = false;
			_changeIsDeletedSubscriber = new ChangeIsDeletedSubscriber(this);
			ServiceFactoryBase.Events.GetEvent<UpdateFilterEvent>().Unsubscribe(OnUpdateFilter);
			ServiceFactoryBase.Events.GetEvent<UpdateFilterEvent>().Subscribe(OnUpdateFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings, CanShowSettings);
			_updateOrganisationDoorsEventSubscriber = new UpdateOrganisationDoorsEventSubscriber<ScheduleViewModel>(this);
		}

		UpdateOrganisationDoorsEventSubscriber<ScheduleViewModel> _updateOrganisationDoorsEventSubscriber;

		public void Initialize()
		{
			var filter = new ScheduleFilter
			{
				UserUID = FiresecManager.CurrentUser.UID,
				LogicalDeletationType = LogicalDeletationType
			};
			Initialize(filter);
		}

		protected override void OnEditOrganisation(Organisation newOrganisation)
		{
			if (_isInitialized)
				base.OnEditOrganisation(newOrganisation);
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			if (_isInitialized)
				base.OnOrganisationUsersChanged(newOrganisation);
		}

		protected override void OnRemoveOrganisation(Guid organisationUID)
		{
			if (_isInitialized)
				base.OnRemoveOrganisation(organisationUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
		}

		public void Select(Guid scheduleUID)
		{
			if (scheduleUID != Guid.Empty)
			{
				var scheduleViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == scheduleUID);
				if (scheduleViewModel != null)
					scheduleViewModel.ExpandToThis();
				SelectedItem= scheduleViewModel;
			}
		}

		protected override void UpdateSelected()
		{
			SelectedItem.Initialize();
		}

		protected override Schedule CopyModel(Schedule source)
		{
			var copy = base.CopyModel(source);
			copy.ScheduleSchemeUID = source.ScheduleSchemeUID;
			copy.IsIgnoreHoliday = source.IsIgnoreHoliday;
			copy.IsOnlyFirstEnter = source.IsOnlyFirstEnter;
			copy.AllowedEarlyLeave = source.AllowedEarlyLeave;
			copy.AllowedLate = source.AllowedLate;

			foreach (var scheduleZone in source.Zones)
				copy.Zones.Add(new ScheduleZone
				{
					UID = Guid.NewGuid(),
					ScheduleUID = copy.UID,
					ZoneUID = scheduleZone.ZoneUID,
				});

			return copy;
		}

		protected override void Remove()
		{
			var isAnyEmployees = EmployeeHelper.Get(new EmployeeFilter { ScheduleUIDs = new List<Guid> { SelectedItem.Model.UID } }).Count() != 0;
			if (!isAnyEmployees || (MessageBoxService.ShowQuestion(CommonViewModels.ScheduleWithEmployee)))
			{
				base.Remove();
			}
		}

		protected override bool Add(Schedule item)
		{
			return ScheduleHelper.Save(item, true);
		}

		protected override IEnumerable<Schedule> GetModels(ScheduleFilter filter)
		{
			return ScheduleHelper.Get(filter);
		}

		protected override IEnumerable<Schedule> GetModelsByOrganisation(Guid organisauinUID)
		{
			return ScheduleHelper.GetByOrganisation(organisauinUID);
		}

		protected override bool MarkDeleted(Schedule model)
		{
			return ScheduleHelper.MarkDeleted(model);
		}

		protected override bool Restore(Schedule model)
		{
			return ScheduleHelper.Restore(model);
		}

		protected override string ItemRemovingName
		{
			get { return CommonResources.WorkSchedule.ToLower(); }
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit; }
		}

		void OnUpdateFilter(HRFilter hrFilter)
		{
			var filter = new ScheduleFilter
			{
				UserUID = FiresecManager.CurrentUser.UID,
				LogicalDeletationType = LogicalDeletationType,
				EmployeeUIDs = hrFilter.EmplooyeeUIDs
			};
			Initialize(filter);
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var nightSettingsViewModel = new NightSettingsViewModel(ParentOrganisation.Organisation.UID);

			if (DialogService.ShowModalWindow(nightSettingsViewModel) && SelectedItem != null)
			{
				if (SelectedItem.IsOrganisation)
					SelectedItem.Update();
				else
				{
					var parentOrganisation = SelectedItem.GetAllParents().FirstOrDefault(x => x.IsOrganisation);
					if (parentOrganisation != null) parentOrganisation.Update();
				}
			}
		}
		bool CanShowSettings()
		{
			return ParentOrganisation != null && !ParentOrganisation.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Holidays_Edit);
		}

		public List<ScheduleViewModel> DoorsParents
		{
			get { return Organisations.SelectMany(x => x.Children).ToList(); }
		}
	}
}