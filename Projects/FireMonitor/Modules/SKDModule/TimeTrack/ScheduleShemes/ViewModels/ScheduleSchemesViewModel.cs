using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemesViewModel : OrganisationBaseViewModel<ScheduleScheme, ScheduleSchemeFilter, ScheduleSchemeViewModel, ScheduleSchemeDetailsViewModel>, ISelectable<Guid>
	{
		bool _isInitialized;
		private Dictionary<Guid, ObservableCollection<DayInterval>> _dayIntervals;
        
		public ScheduleSchemesViewModel()
			:base()
		{
			_isInitialized = false;
		}

		public void Initialize()
		{
			var filter = new ScheduleSchemeFilter() { UserUID = FiresecManager.CurrentUser.UID };
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
				_dayIntervals.Values.ForEach(item => item.Insert(0, new DayInterval()
				{
					UID = Guid.Empty,
					Name = "Никогда",
				}));
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
				if (!day.IsDeleted)
				{
					copy.DayIntervals.Add(new ScheduleDayInterval()
					{
						DayIntervalUID = day.DayIntervalUID,
						Number = day.Number,
						ScheduleSchemeUID = copy.UID,
					});
				}
			}
			return copy;
		}

		protected override IEnumerable<ScheduleScheme> GetModels(ScheduleSchemeFilter filter)
		{
			return ScheduleSchemaHelper.Get(filter);
		}
		protected override IEnumerable<ScheduleScheme> GetModelsByOrganisation(Guid organisationUID)
		{
			return ScheduleSchemaHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(Guid uid)
		{
			return ScheduleSchemaHelper.MarkDeleted(uid);
		}
		protected override bool Save(ScheduleScheme item)
		{
			return ScheduleSchemaHelper.Save(item);
		}

        protected override string ItemRemovingName
        {
            get { return "график"; }
        }
	}
}