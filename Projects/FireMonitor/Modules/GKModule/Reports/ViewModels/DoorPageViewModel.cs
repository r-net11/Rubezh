using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using RubezhAPI.SKD.ReportFilters;
using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using Infrastructure.Common;
using Common;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DoorPageViewModel : FilterContainerViewModel
	{
		public DoorPageViewModel()
		{
			Title = "Точки доступа";
			Doors = new ObservableCollection<CheckedItemViewModel<GKDoor>>(GKManager.Doors.Select(item => new CheckedItemViewModel<GKDoor>(item)));
			SelectAllCommand = new RelayCommand(() => Doors.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Doors.ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<GKDoor>> Doors { get; private set; }

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