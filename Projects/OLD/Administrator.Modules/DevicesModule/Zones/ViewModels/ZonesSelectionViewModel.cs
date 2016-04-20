using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	[SaveSizeAttribute]
	public class ZonesSelectionViewModel : SaveCancelDialogViewModel
	{
		public List<Guid> Zones { get; private set; }

		public ZonesSelectionViewModel(Device device, List<Guid> zones, ZoneLogicState zoneLogicState)
		{
			Title = "Выбор зон устройства " + device.PresentationAddressAndName;
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
			Zones = zones;
			TargetZones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			var zoneTypeFilter = ZoneType.Fire;
			switch (zoneLogicState)
			{
				case ZoneLogicState.Alarm:
				case ZoneLogicState.GuardSet:
				case ZoneLogicState.GuardUnSet:
				case ZoneLogicState.PCN:
				case ZoneLogicState.Lamp:
					zoneTypeFilter = ZoneType.Guard;
					break;
			}

			List<Zone> availableZones = FiresecManager.FiresecConfiguration.GetChannelZones(device);
			if (device.Driver.DriverType == DriverType.Exit)
			{
				availableZones = FiresecManager.FiresecConfiguration.GetPanelZones(device);
			}

			foreach (var zone in availableZones)
			{
				var zoneViewModel = new ZoneViewModel(zone);

				if (zone.ZoneType != zoneTypeFilter)
				{
					continue;
				}

				if ((zoneLogicState == ZoneLogicState.MPTAutomaticOn) || (zoneLogicState == ZoneLogicState.MPTOn) || (zoneLogicState == ZoneLogicState.Firefighting))
				{
					if (!zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.MPT))
						continue;

					//if (device.ParentPanel.Children.Any(x => x.Driver.DriverType == DriverType.MPT && x.ZoneUID == zone.UID) == false)
					//{
					//	continue;
					//}
				}

				if (Zones.Contains(zone.UID))
					TargetZones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public ZonesSelectionViewModel(Device device, List<Guid> zones)
		{
			Title = "Выбор зон индикатора";

			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAdd);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemove);

			Zones = zones;
			TargetZones = new ObservableCollection<ZoneViewModel>();
			SourceZones = new ObservableCollection<ZoneViewModel>();

			foreach (var zone in FiresecManager.FiresecConfiguration.GetChannelZones(device))
			{
				var zoneViewModel = new ZoneViewModel(zone);

				if (Zones.Contains(zone.UID))
					TargetZones.Add(zoneViewModel);
				else
					SourceZones.Add(zoneViewModel);
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public ObservableCollection<ZoneViewModel> SourceZones { get; private set; }

   		ZoneViewModel _selectedSourceZone;
		public ZoneViewModel SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged(() => SelectedSourceZone);
			}
		}

		public ObservableCollection<ZoneViewModel> TargetZones { get; private set; }

		ZoneViewModel _selectedTargetZone;
		public ZoneViewModel SelectedTargetZone
		{
			get { return _selectedTargetZone; }
			set
			{
				_selectedTargetZone = value;
				OnPropertyChanged(() => SelectedTargetZone);
			}
		}

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceZones;

		void OnAdd(object parameter)
		{
			var index = SourceZones.IndexOf(SelectedSourceZone);

			SelectedSourceZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedSourceZones)
			{
				ZoneViewModel zoneViewModel = selectedZone as ZoneViewModel;
				if(zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				TargetZones.Add(zoneViewModel);
				SourceZones.Remove(zoneViewModel);
			}

			OnPropertyChanged(() => SourceZones);

			index = Math.Min(index, SourceZones.Count - 1);
			if (index > -1)
				SelectedSourceZone = SourceZones[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetZones;
		void OnRemove(object parameter)
		{
			var index = TargetZones.IndexOf(SelectedTargetZone);

			SelectedTargetZones = (IList)parameter;
			var zoneViewModels = new List<ZoneViewModel>();
			foreach (var selectedZone in SelectedTargetZones)
			{
				ZoneViewModel zoneViewModel = selectedZone as ZoneViewModel;
				if(zoneViewModel != null)
					zoneViewModels.Add(zoneViewModel);
			}
			foreach (var zoneViewModel in zoneViewModels)
			{
				SourceZones.Add(zoneViewModel);
				TargetZones.Remove(zoneViewModel);
			}

			OnPropertyChanged(() => TargetZones);
			SelectedTargetZone = TargetZones.FirstOrDefault();

			index = Math.Min(index, TargetZones.Count - 1);
			if (index > -1)
				SelectedTargetZone = TargetZones[index];
		}


		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var zoneViewModel in SourceZones)
			{
				TargetZones.Add(zoneViewModel);
			}
			SourceZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var zoneViewModel in TargetZones)
			{
				SourceZones.Add(zoneViewModel);
			}
			TargetZones.Clear();
			SelectedTargetZone = TargetZones.FirstOrDefault();
		}

		bool CanAdd(object parameter)
		{
			return SelectedSourceZone != null;
		}

		bool CanRemove(object parameter)
		{
			return SelectedTargetZone != null;
		}

		public bool CanAddAll()
		{
			return (SourceZones.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (TargetZones.Count > 0);
		}

		protected override bool Save()
		{
			Zones = new List<Guid>(TargetZones.Select(x => x.Zone.UID));
			return base.Save();
		}
	}
}