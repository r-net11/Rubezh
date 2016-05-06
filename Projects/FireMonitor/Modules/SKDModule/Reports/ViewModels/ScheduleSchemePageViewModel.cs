using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using RubezhAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Common;
using RubezhClient.SKDHelpers;
using RubezhAPI.SKD;
using Infrastructure.Common.TreeList;

namespace SKDModule.Reports.ViewModels
{
	public class ScheduleSchemePageViewModel : FilterContainerViewModel
	{
		public ScheduleSchemePageViewModel()
		{
			Title = "Графики работы";
			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			Schedules = new ObservableCollection<TreeNodeItemViewModel>(organisations.Select(item => new TreeNodeItemViewModel(item, false)));
			var scheduleSchemas = ScheduleHelper.Get(new ScheduleFilter());
			if (scheduleSchemas != null)
			{
				foreach (var scheduleScheme in scheduleSchemas)
				{
					var schedule = Schedules.FirstOrDefault(x => x.Item.UID == scheduleScheme.OrganisationUID);
					if (schedule != null)
						schedule.AddChild(new TreeNodeItemViewModel(scheduleScheme, true));
				}
			}
			SelectAllCommand = new RelayCommand(() => Schedules.SelectMany(item => item.Children).ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Schedules.SelectMany(item => item.Children).ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<TreeNodeItemViewModel> Schedules { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterScheduleScheme;
			if (scheduleFilter == null)
				return;
			if (scheduleFilter.ScheduleSchemas == null)
				scheduleFilter.ScheduleSchemas = new List<Guid>();
			Schedules.ForEach(item => item.IsExpanded = true);
			Schedules.SelectMany(item => item.Children).ForEach(item => item.IsChecked = scheduleFilter.ScheduleSchemas.Contains(item.Item.UID));
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var scheduleFilter = filter as IReportFilterScheduleScheme;
			if (scheduleFilter == null)
				return;
			scheduleFilter.ScheduleSchemas = Schedules.SelectMany(item => item.Children).Where(item => item.IsChecked).Select(item => item.Item.UID).ToList();
		}
	}
}