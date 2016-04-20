using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesViewModel : OrganisationBaseViewModel<ScheduleScheme, ScheduleSchemeFilter, ScheduleSchemeViewModel, ScheduleSchemeDetailsViewModel>, ISelectable<Guid>
	{
		private Dictionary<Guid, ObservableCollection<DayInterval>> _dayIntervals;
		public static ScheduleSchemesViewModel Current { get; private set; }
		
		public ScheduleSchemesViewModel()
			:base()
		{
			Current = this;
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Unsubscribe(OnEditDayInterval);
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Subscribe(OnEditDayInterval);
		}

		public override void Initialize(ScheduleSchemeFilter filter)
		{
			base.Initialize(filter);
			ReloadDayIntervals();
		}

		public void ReloadDayIntervals()
		{
			if (Organisations != null && Organisations.Count > 0)
			{
				var dayIntervals = DayIntervalHelper.Get(new DayIntervalFilter()
				{
					UserUID = ClientManager.CurrentUser.UID,
					OrganisationUIDs = Organisations.Select(item => item.Organisation.UID).ToList(),
				});
				_dayIntervals = new Dictionary<Guid, ObservableCollection<DayInterval>>();
				Organisations.ForEach(item => _dayIntervals.Add(item.Organisation.UID, new ObservableCollection<DayInterval>()));
				foreach (var item in dayIntervals)
				{
					var organisation = _dayIntervals[item.OrganisationUID];
					if (organisation != null)
						organisation.Add(item);
				}
			}
		}
		public ObservableCollection<DayInterval> GetDayIntervals(Guid organisationUID)
		{
			return _dayIntervals.ContainsKey(organisationUID) ? _dayIntervals[organisationUID] : new ObservableCollection<DayInterval>();
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

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit; }
		}

		void OnEditDayInterval(Guid dayInternalUID)
		{
			ReloadDayIntervals();
			SelectedItem = Organisations.FirstOrDefault();
		}

		protected override void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
			base.OnOrganisationUsersChanged(newOrganisation);
			ReloadDayIntervals();
		}
	}
}