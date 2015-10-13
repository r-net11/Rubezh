using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.ViewModels
{
	public class SchedulePageViewModel : FilterContainerViewModel
	{
		public SchedulePageViewModel()
		{
			Title = "График";
			SelectAllCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Schedules.ForEach(item => item.IsChecked = false));

			Schedules = new ObservableCollection<CheckedItemViewModel<CommonScheduleViewModel>>();
			var schedules = GKScheduleHelper.GetSchedules();
			if (schedules == null)
				schedules = new List<GKSchedule>();
			foreach (var schedule in schedules.OrderBy(x => x.No))
			{
				var scheduleViewModel = new CommonScheduleViewModel(schedule);
				Schedules.Add(new CheckedItemViewModel<CommonScheduleViewModel>(scheduleViewModel));
			}
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<CommonScheduleViewModel>> Schedules { get; private set; }

		bool _scheduleEnter;
		public bool ScheduleEnter
		{
			get { return _scheduleEnter; }
			set
			{
				_scheduleEnter = value;
				OnPropertyChanged(() => ScheduleEnter);
			}
		}
		bool _scheduleExit;
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
			Schedules.ForEach(item => item.IsChecked = scheduleFilter.Schedules.Contains(item.Item.No));
			ScheduleEnter = scheduleFilter.ScheduleEnter;
			ScheduleExit = scheduleFilter.ScheduleExit;
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterSchedule;
			if (scheduleFilter == null)
				return;
			scheduleFilter.Schedules = Schedules.Where(item => item.IsChecked).Select(item => item.Item.No).ToList();
			scheduleFilter.ScheduleEnter = ScheduleEnter;
			scheduleFilter.ScheduleExit = ScheduleExit;
		}
	}

	public class CommonScheduleViewModel
	{
		public int No { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public CommonScheduleViewModel(GKSchedule gkSchedule)
		{
			No = gkSchedule.No;
			Name = gkSchedule.Name;
			Description = gkSchedule.Description;
		}
	}
}