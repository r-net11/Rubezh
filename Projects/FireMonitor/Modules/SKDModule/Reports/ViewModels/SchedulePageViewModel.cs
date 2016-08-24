using Common;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.Reports.ViewModels
{
	public class SchedulePageViewModel : FilterContainerViewModel
	{
		public SchedulePageViewModel()
		{
			Title = CommonResources.Schedule;
			SelectAllCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = false));

			Schedules = new ObservableCollection<CheckedItemViewModel<CommonScheduleViewModel>>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				var scheduleViewModel = new CommonScheduleViewModel(weeklyInterval);
				Schedules.Add(new CheckedItemViewModel<CommonScheduleViewModel>(scheduleViewModel));
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<CommonScheduleViewModel>> Schedules { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterSchedule;
			if (scheduleFilter == null)
				return;
			if (scheduleFilter.Schedules == null)
				scheduleFilter.Schedules = new List<int>();
			Schedules.ForEach(item => item.IsChecked = scheduleFilter.Schedules.Contains(item.Item.No));
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterSchedule;
			if (scheduleFilter == null)
				return;
			scheduleFilter.Schedules = Schedules.Where(item => item.IsChecked).Select(item => item.Item.No).ToList();
		}
	}

	public class CommonScheduleViewModel
	{
		public int No { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public CommonScheduleViewModel(SKDWeeklyInterval strazhSchedule)
		{
			No = strazhSchedule.ID;
			Name = strazhSchedule.Name;
			Description = strazhSchedule.Description;
		}

		public CommonScheduleViewModel(Schedule skdSchedule)
		{
			Name = skdSchedule.Name;
			Description = skdSchedule.Description;
		}
	}
}