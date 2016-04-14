using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class SchedulesViewModel 
		: OrganisationBaseViewModel<Schedule, ScheduleFilter, ScheduleViewModel, ScheduleDetailsViewModel>, ISelectable<Guid>, ICardDoorsParentList<ScheduleViewModel>
	{
		public SchedulesViewModel()
			:base()
		{
			ServiceFactory.Events.GetEvent<UpdateFilterEvent>().Unsubscribe(OnUpdateFilter);
			ServiceFactory.Events.GetEvent<UpdateFilterEvent>().Subscribe(OnUpdateFilter);
			_updateOrganisationDoorsEventSubscriber = new UpdateOrganisationDoorsEventSubscriber<ScheduleViewModel>(this);
		}

		UpdateOrganisationDoorsEventSubscriber<ScheduleViewModel> _updateOrganisationDoorsEventSubscriber;

		public void Select(Guid scheduleUID)
		{
			if (!IsConnected)
				return;
			if (scheduleUID != Guid.Empty)
			{
				var scheduleViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == scheduleUID);
				if (scheduleViewModel != null)
					scheduleViewModel.ExpandToThis();
				SelectedItem = scheduleViewModel;
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
				copy.Zones.Add(new ScheduleZone()
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
			if (!isAnyEmployees || (isAnyEmployees && MessageBoxService.ShowQuestion("Существуют привязанные к графику сотрудники. Продолжить?")))
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
			get { return "график работы"; }
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit; }
		}

		void OnUpdateFilter(HRFilter hrFilter)
		{
			var filter = new ScheduleFilter()
			{
				UserUID = ClientManager.CurrentUser.UID,
				LogicalDeletationType = hrFilter.LogicalDeletationType,
				EmployeeUIDs = hrFilter.EmplooyeeUIDs
			};
			Initialize(filter);
		}
		public List<ScheduleViewModel> DoorsParents
		{
			get { return Organisations.SelectMany(x => x.Children).ToList(); }
		}
	}
}