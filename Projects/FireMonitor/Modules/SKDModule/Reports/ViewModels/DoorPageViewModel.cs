using Common;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.Reports.ViewModels
{
	public class DoorPageViewModel : FilterContainerViewModel
	{
		public DoorPageViewModel()
		{
			Title = CommonResources.Doors;
			Doors = new ObservableCollection<CheckedItemViewModel<SKDDoor>>(SKDManager.Doors.Select(item => new CheckedItemViewModel<SKDDoor>(item)));
			SelectAllCommand = new RelayCommand(() => Doors.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Doors.ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<SKDDoor>> Doors { get; private set; }

		public override void LoadFilter(SKDReportFilter filter)
		{
			var doorFilter = filter as IReportFilterDoor;
			if (doorFilter == null)
				return;
			if (doorFilter.Doors == null)
				doorFilter.Doors = new List<Guid>();
			Doors.ForEach(item => item.IsChecked = doorFilter.Doors.Contains(item.Item.UID));
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var doorFilter = filter as IReportFilterDoor;
			if (doorFilter == null)
				return;
			doorFilter.Doors = Doors.Where(item => item.IsChecked).Select(item => item.Item.UID).ToList();
		}
	}
}
