using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.SKDReports;
using RubezhAPI.SKD.ReportFilters;
using SKDModule.ViewModels;
using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace SKDModule.Reports.ViewModels
{
	public class ZonePageViewModel : FilterContainerViewModel
	{
		public ZonePageViewModel()
		{
			Title = "Зоны";
			Zones = new ObservableCollection<CheckedItemViewModel<CommonZoneViewModel>>();
			foreach (var zone in GKManager.SKDZones)
			{
				var zoneViewModel = new CommonZoneViewModel(zone);
				Zones.Add(new CheckedItemViewModel<CommonZoneViewModel>(zoneViewModel));
			}
			SelectAllCommand = new RelayCommand(() => Zones.ForEach(item => item.IsChecked = true));
			SelectNoneCommand = new RelayCommand(() => Zones.ForEach(item => item.IsChecked = false));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<CheckedItemViewModel<CommonZoneViewModel>> Zones { get; private set; }

		bool _withDirection;
		public bool WithDirection
		{
			get { return _withDirection; }
			set
			{
				_withDirection = value;
				OnPropertyChanged(() => WithDirection);
			}
		}
		bool _zoneIn;
		public bool ZoneIn
		{
			get { return _zoneIn; }
			set
			{
				_zoneIn = value;
				OnPropertyChanged(() => ZoneIn);
			}
		}
		bool _zoneOut;
		public bool ZoneOut
		{
			get { return _zoneOut; }
			set
			{
				_zoneOut = value;
				OnPropertyChanged(() => ZoneOut);
			}
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var zoneFilter = filter as IReportFilterZone;
			if (zoneFilter == null)
				return;
			if (zoneFilter.Zones == null)
				zoneFilter.Zones = new List<Guid>();
			Zones.ForEach(item => item.IsChecked = zoneFilter.Zones.Contains(item.Item.UID));
			WithDirection = zoneFilter is IReportFilterZoneWithDirection;
			if (WithDirection)
			{
				ZoneIn = ((IReportFilterZoneWithDirection)zoneFilter).ZoneIn;
				ZoneOut = ((IReportFilterZoneWithDirection)zoneFilter).ZoneOut;
			}
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var zoneFilter = filter as IReportFilterZone;
			if (zoneFilter == null)
				return;
			zoneFilter.Zones = Zones.Where(item => item.IsChecked).Select(item => item.Item.UID).ToList();
			var direction = zoneFilter as IReportFilterZoneWithDirection;
			if (direction != null)
			{
				direction.ZoneIn = ZoneIn;
				direction.ZoneOut = ZoneOut;
			}
		}
	}

	public class CommonZoneViewModel
	{
		public Guid UID { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public CommonZoneViewModel(GKSKDZone gkZone)
		{
			UID = gkZone.UID;
			Name = gkZone.Name;
			Description = gkZone.Description;
		}
	}
}