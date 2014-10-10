using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SchedulesViewModel : OrganisationBaseViewModel<Schedule, ScheduleFilter, ScheduleViewModel, ScheduleDetailsViewModel>, ISelectable<Guid>, ITimeTrackItemsViewModel
	{
		bool _isInitialized;
		ChangeIsDeletedSubscriber _changeIsDeletedSubscriber;
		public LogicalDeletationType LogicalDeletationType { get; set; }
		
		public SchedulesViewModel()
			:base()
		{
			_isInitialized = false;
			_changeIsDeletedSubscriber = new ChangeIsDeletedSubscriber(this);
		}

		public void Initialize()
		{
			var filter = new ScheduleFilter()
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
			foreach (var scheduleZone in source.Zones)
				copy.Zones.Add(new ScheduleZone()
				{
					ScheduleUID = copy.UID,
					ZoneUID = scheduleZone.ZoneUID,
				});
			return copy;
		}

		protected override void Remove()
		{
			var isAnyEmployees = EmployeeHelper.Get(new EmployeeFilter { ScheduleUIDs = new List<Guid> { SelectedItem.Model.UID } }).Count() == 0;
			if (MessageBoxService.ShowQuestion("Существуют привязанные к графику сотрудники. Продолжить?"))
			{
				base.Remove();
			}
		}

		protected override bool Save(Schedule item)
		{
			return ScheduleHelper.Save(item);
		}

		protected override IEnumerable<Schedule> GetModels(ScheduleFilter filter)
		{
			return ScheduleHelper.Get(filter);
		}

		protected override IEnumerable<Schedule> GetModelsByOrganisation(Guid organisauinUID)
		{
			return ScheduleHelper.GetByOrganisation(organisauinUID);
		}

		protected override bool MarkDeleted(Guid uid)
		{
			return ScheduleHelper.MarkDeleted(uid);
		}

		protected override bool Restore(Guid uid)
		{
			return ScheduleHelper.Restore(uid);
		}

		protected override string ItemRemovingName
		{
			get { return "график работы"; }
		}
	}
}