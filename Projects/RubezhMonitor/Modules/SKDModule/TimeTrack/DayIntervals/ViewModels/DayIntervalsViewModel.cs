using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class DayIntervalsViewModel : OrganisationBaseViewModel<DayInterval, DayIntervalFilter, DayIntervalViewModel, DayIntervalDetailsViewModel>, ISelectable<Guid>
	{
		public override void Initialize(DayIntervalFilter filter)
		{
			base.Initialize(filter);
			var isAdd = false;
			foreach (var organisation in Organisations)
			{
				var dayoff = organisation.Children.FirstOrDefault(x => x.Name == "Выходной");
				if (dayoff == null)
				{
					var interval = new DayInterval()
					{
						Name = "Выходной",
						DayIntervalParts = new List<DayIntervalPart>(),
						UID = Guid.NewGuid(),
						OrganisationUID = organisation.UID
					};
					Add(interval);
					isAdd = true;
				}
				else
				{
					organisation.RemoveChild(dayoff);
					organisation.AddChildFirst(dayoff);
				}
			}
			if (isAdd) 
				base.Initialize(filter);
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
				MessageBoxService.ShowQuestion("Существуют графики работы, содержашие данный дневной график. Продолжить?"))
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
					TransitionType = item.TransitionType,
					Number = item.Number
				};
				copy.DayIntervalParts.Add(dayIntervalPart);
				copy.SlideTime = source.SlideTime;
			}
			return copy;
		}

		protected override void AfterRemove(DayInterval model)
		{
			base.AfterRemove(model);
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(model.UID);
		}

		protected override void AfterRestore(DayInterval model)
		{
			base.AfterRestore(model);
			ServiceFactory.Events.GetEvent<EditDayIntervalEvent>().Publish(model.UID);
		}

		protected override string ItemRemovingName
		{
			get { return "дневной график"; }
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DaySchedules_Edit; }
		}

		protected override bool CanEdit()
        {
            return base.CanEdit() && SelectedItem.Name != "Выходной";
        }
        protected override bool CanCopy()
        {
            return base.CanCopy() && SelectedItem.Name != "Выходной";
        }
        protected override bool CanRemove()
        {
            return base.CanRemove() && SelectedItem.Name != "Выходной";
        }
	}
}