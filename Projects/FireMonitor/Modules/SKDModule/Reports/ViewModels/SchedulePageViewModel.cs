﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using FiresecAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Common;
using FiresecClient.SKDHelpers;
using FiresecAPI.SKD;

namespace SKDModule.Reports.ViewModels
{
	public class SchedulePageViewModel : FilterContainerViewModel
	{
		public SchedulePageViewModel()
		{
			Title = "График";
			Schedules = new ObservableCollection<CheckedItemViewModel<SKDWeeklyInterval>>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				var viewModel = new CheckedItemViewModel<SKDWeeklyInterval>(weeklyInterval);
				Schedules.Add(viewModel);
			}
			SelectAllCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<SKDWeeklyInterval>> Schedules { get; private set; }

		private bool _withDirection;
		public bool WithDirection
		{
			get { return _withDirection; }
			set
			{
				_withDirection = value;
				OnPropertyChanged(() => WithDirection);
			}
		}
		private bool _scheduleEnter;
		public bool ScheduleEnter
		{
			get { return _scheduleEnter; }
			set
			{
				_scheduleEnter = value;
				OnPropertyChanged(() => ScheduleEnter);
			}
		}
		private bool _scheduleExit;
		public bool ScheduleExit
		{
			get { return _scheduleExit; }
			set
			{
				_scheduleExit = value;
				OnPropertyChanged(() => ScheduleExit);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterSchedule;
			if (scheduleFilter == null)
				return;
			if (scheduleFilter.Schedules == null)
				scheduleFilter.Schedules = new List<int>();
			Schedules.ForEach(item => item.IsChecked = scheduleFilter.Schedules.Contains(item.Item.ID));
			WithDirection = scheduleFilter is IReportFilterScheduleWithDirection;
			if (WithDirection)
			{
				ScheduleEnter = ((IReportFilterScheduleWithDirection)scheduleFilter).ScheduleEnter;
				ScheduleExit = ((IReportFilterScheduleWithDirection)scheduleFilter).ScheduleExit;
			}
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterSchedule;
			if (scheduleFilter == null)
				return;
			scheduleFilter.Schedules = Schedules.Where(item => item.IsChecked).Select(item => item.Item.ID).ToList();
			var direction = scheduleFilter as IReportFilterScheduleWithDirection;
			if (direction != null)
			{
				direction.ScheduleEnter = ScheduleEnter;
				direction.ScheduleExit = ScheduleExit;
			}
		}
	}
}
