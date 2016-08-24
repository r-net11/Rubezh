using Localization.SKD.Common;
using Localization.SKD.ViewModels;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class DayIntervalsViewModel : OrganisationBaseViewModel<DayInterval, DayIntervalFilter, DayIntervalViewModel, DayIntervalDetailsViewModel>, ISelectable<Guid>, ITimeTrackItemsViewModel
	{
		bool _isInitialized;

		public DayIntervalsViewModel()
		{
			_isInitialized = false;
			_changeIsDeletedSubscriber = new ChangeIsDeletedSubscriber(this);
		}

		public LogicalDeletationType LogicalDeletationType { get; set; }
		ChangeIsDeletedSubscriber _changeIsDeletedSubscriber;

		public override void OnShow()
		{
			base.OnShow();
			if (!_isInitialized)
			{
				Initialize();
				_isInitialized = true;
			}
		}

		public void Initialize()
		{
			var filter = new DayIntervalFilter { UserUID = FiresecManager.CurrentUser.UID, LogicalDeletationType = LogicalDeletationType };
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

		public void Select(Guid dayIntervalUID)
		{
			if (dayIntervalUID != Guid.Empty)
			{
				var dayIntervalViewModel = Organisations.SelectMany(x => x.Children).FirstOrDefault(x => x.Model != null && x.Model.UID == dayIntervalUID);
				if (dayIntervalViewModel != null)
					dayIntervalViewModel.ExpandToThis();
				SelectedItem = dayIntervalViewModel;
			}
		}

		protected override void Remove()
		{
			if (ScheduleSchemeHelper.Get(new ScheduleSchemeFilter { DayIntervalUIDs = new List<Guid> { SelectedItem.Model.UID } }).Count() == 0 ||
				MessageBoxService.ShowQuestion(CommonViewModels.WorkSchedulesWithDaySchedules))
			{
				base.Remove();
			}
		}

		protected override void UpdateSelected()
		{
			SelectedItem.Initialize();
		}

		protected override IEnumerable<DayInterval> GetModels(DayIntervalFilter filter)
		{
			return DayIntervalHelper.Get(filter);
		}
		protected override IEnumerable<DayInterval> GetModelsByOrganisation(Guid organisationUID)
		{
			return DayIntervalHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(DayInterval model)
		{
			return DayIntervalHelper.MarkDeleted(model);
		}
		protected override bool Restore(DayInterval model)
		{
			return DayIntervalHelper.Restore(model);
		}
		protected override bool Add(DayInterval item)
		{
			return DayIntervalHelper.Save(item, true);
		}

		protected override DayInterval CopyModel(DayInterval source)
		{
			var copy = base.CopyModel(source);
			foreach (var item in source.DayIntervalParts)
			{
				var dayIntervalPart = new DayIntervalPart
				{
					DayIntervalUID = copy.UID,
					BeginTime = item.BeginTime,
					EndTime = item.EndTime,
					TransitionType = item.TransitionType
				};
				copy.DayIntervalParts.Add(dayIntervalPart);
				copy.SlideTime = source.SlideTime;
			}
			return copy;
		}

		protected override void AfterRemove(DayInterval model)
		{
			base.AfterRemove(model);
			ServiceFactoryBase.Events.GetEvent<EditDayIntervalEvent>().Publish(model.UID);
		}

		protected override void AfterRestore(DayInterval model)
		{
			base.AfterRestore(model);
			ServiceFactoryBase.Events.GetEvent<EditDayIntervalEvent>().Publish(model.UID);
		}

		protected override string ItemRemovingName
		{
			get { return CommonResources.DaySchedule.ToLower(); }
		}

		protected override StrazhAPI.Models.PermissionType Permission
		{
			get { return StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit; }
		}


	}
}