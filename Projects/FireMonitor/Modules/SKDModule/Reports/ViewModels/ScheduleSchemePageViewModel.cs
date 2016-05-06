using Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.Reports.ViewModels
{
	public class ScheduleSchemePageViewModel : FilterContainerViewModel
	{
		public ScheduleSchemePageViewModel()
		{
			Title = "Графики работы";

			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			Schedules = new ObservableCollection<TreeNodeItemViewModel>(organisations.Select(item => new TreeNodeItemViewModel(item, false)));
			var map = Schedules.ToDictionary(item => item.Item.UID);

			var scheduleSchemas = ScheduleHelper.Get(new ScheduleFilter());
			scheduleSchemas.ForEach(item => map[item.OrganisationUID].AddChild(new TreeNodeItemViewModel(item, true)));

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
