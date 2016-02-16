using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesViewModel : OrganisationBaseViewModel<ScheduleScheme, ScheduleSchemeFilter, ScheduleSchemeViewModel, ScheduleSchemeDetailsViewModel>, ISelectable<Guid>, ITimeTrackItemsViewModel
	{
		bool _isInitialized;
		private Dictionary<Guid, ObservableCollection<DayInterval>> _dayIntervals;
		
		public ScheduleSchemesViewModel()
			:base()
		{
			_isInitialized = false;
			_changeIsDeletedSubscriber = new ChangeIsDeletedSubscriber(this);
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Unsubscribe(OnEditDayInterval);
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Subscribe(OnEditDayInterval);
		}

		public LogicalDeletationType LogicalDeletationType { get; set; }
		ChangeIsDeletedSubscriber _changeIsDeletedSubscriber;

		public void Initialize()
		{
			var filter = new ScheduleSchemeFilter() { UserUID = FiresecManager.CurrentUser.UID, LogicalDeletationType = LogicalDeletationType };
			Initialize(filter);
		}

		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
			ReloadDayIntervals();
		}

		protected override void OnEditOrganisation(Organisation newOrganisation)
		{
			if(_isInitialized)
				base.OnEditOrganisation(newOrganisation);
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			if(_isInitialized)
				base.OnOrganisationUsersChanged(newOrganisation);
		}

		protected override void OnRemoveOrganisation(Guid organisationUID)
		{
			if (_isInitialized)
				base.OnRemoveOrganisation(organisationUID);
		}

		public void ReloadDayIntervals()
		{
			if (Organisations != null)
			{
				var dayIntervals = DayIntervalHelper.Get(new DayIntervalFilter()
				{
					UserUID = FiresecManager.CurrentUser.UID,
					OrganisationUIDs = Organisations.Select(item => item.Organisation.UID).ToList(),
				});
				_dayIntervals = new Dictionary<Guid, ObservableCollection<DayInterval>>();
				Organisations.ForEach(item => _dayIntervals.Add(item.Organisation.UID, new ObservableCollection<DayInterval>()));
				dayIntervals.ForEach(item => _dayIntervals[item.OrganisationUID].Add(item));
				_dayIntervals.Values.ForEach(item => item.Insert(0, DayIntervalCreator.CreateDayIntervalNever()));
			}
		}
		public ObservableCollection<DayInterval> GetDayIntervals(Guid organisationUID)
		{
			return _dayIntervals.ContainsKey(organisationUID) ? _dayIntervals[organisationUID] : new ObservableCollection<DayInterval>();
		}
		
		public ObservableCollection<DayInterval> GetDayIntervals(Guid organisationUID, ScheduleSchemeType scheduleSchemeType)
		{
			IEnumerable<DayInterval> dayIntervals = null;
			switch (scheduleSchemeType)
			{
				case ScheduleSchemeType.Week:
					dayIntervals = GetDayIntervals(organisationUID).Where(x => !x.DayIntervalParts.Any(y => y.TransitionType == DayIntervalPartTransitionType.Night));
					break;
				case ScheduleSchemeType.Month:
					dayIntervals = GetDayIntervals(organisationUID).Where(x => x.SlideTime == TimeSpan.Zero && !x.DayIntervalParts.Any(y => y.TransitionType == DayIntervalPartTransitionType.Night));
					break;
				case ScheduleSchemeType.SlideDay:
					dayIntervals = GetDayIntervals(organisationUID).Where(x => x.SlideTime == TimeSpan.Zero);
					break;
			}
			return dayIntervals == null
				? new ObservableCollection<DayInterval>()
				: new ObservableCollection<DayInterval>(dayIntervals);
		}

		public void Select(Guid scheduleSchemelUID)
		{
			if (scheduleSchemelUID != Guid.Empty)
			{
				var scheduleSchemeViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == scheduleSchemelUID);
				if (scheduleSchemeViewModel != null)
					scheduleSchemeViewModel.ExpandToThis();
				SelectedItem = scheduleSchemeViewModel;
			}
		}

		protected override void UpdateSelected()
		{
			SelectedItem.Initialize();
		}

		protected override ScheduleScheme CopyModel(ScheduleScheme source)
		{
			var copy = base.CopyModel(source);
			copy.Type = source.Type;
			foreach (var day in source.DayIntervals)
			{
				copy.DayIntervals.Add(new ScheduleDayInterval()
				{
					DayIntervalUID = day.DayIntervalUID,
					Number = day.Number,
					ScheduleSchemeUID = copy.UID,
				});
			}
			return copy;
		}

		protected override IEnumerable<ScheduleScheme> GetModels(ScheduleSchemeFilter filter)
		{
			return ScheduleSchemeHelper.Get(filter);
		}
		protected override IEnumerable<ScheduleScheme> GetModelsByOrganisation(Guid organisationUID)
		{
			return ScheduleSchemeHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(ScheduleScheme model)
		{
			return ScheduleSchemeHelper.MarkDeleted(model);
		}
		protected override bool Restore(ScheduleScheme model)
		{
			return ScheduleSchemeHelper.Restore(model);
		}
		protected override bool Add(ScheduleScheme item)
		{
			return ScheduleSchemeHelper.Save(item, true);
		}
		protected override string ItemRemovingName
		{
			get { return "график"; }
		}

		protected override void Remove()
		{
			if (ScheduleHelper.Get(new ScheduleFilter { ScheduleSchemeUIDs = new List<Guid> { SelectedItem.Model.UID } }).Count() == 0 ||
				MessageBoxService.ShowQuestion("Существуют графики сотрудников, содержашие данный график работы. Продолжить?"))
			{
				base.Remove();
			}
		}

		protected override FiresecAPI.Models.PermissionType Permission
		{
			get { return FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit; }
		}

		void OnEditDayInterval(Guid dayInternalUID)
		{
			SelectedItem = Organisations.FirstOrDefault();
		}
	}
}