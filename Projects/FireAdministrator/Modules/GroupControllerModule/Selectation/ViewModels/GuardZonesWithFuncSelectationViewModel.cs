using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using Common;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class GuardZonesWithFuncSelectationViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<DeviceGuardZoneViewModel> DeviceGuardZones { get; private set; }
		public bool CanCreateNew { get; private set; }
		GKDevice Device { get; set; }

		public GuardZonesWithFuncSelectationViewModel(GKDevice device, bool canCreateNew = false)
		{
			Title = "Выбор охранных зон";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);
			CreateNewCommand = new RelayCommand(OnCreateNew);

			Device = device;
			DeviceGuardZones = new ObservableCollection<DeviceGuardZoneViewModel>();
			foreach (var zone in device.GuardZones)
			{
				var guardZoneDevice = zone.GuardZoneDevices.FirstOrDefault(x => x.Device == device);
				if (guardZoneDevice != null)
				{
					var deviceGuardZone = new GKDeviceGuardZone();
					deviceGuardZone.GuardZone = zone;
					deviceGuardZone.GuardZoneUID = zone.UID;
					deviceGuardZone.ActionType = guardZoneDevice.ActionType;
					deviceGuardZone.CodeReaderSettings = guardZoneDevice.CodeReaderSettings;
					DeviceGuardZones.Add(new DeviceGuardZoneViewModel(deviceGuardZone, device));
				}
			}

			CanCreateNew = canCreateNew;
			TargetZones = new SortableObservableCollection<DeviceGuardZoneViewModel>();
			SourceZones = new SortableObservableCollection<DeviceGuardZoneViewModel>();

			foreach (var guardZone in GKManager.GuardZones)
			{
				var deviceGuardZone = DeviceGuardZones.FirstOrDefault(x => x.DeviceGuardZone.GuardZone == guardZone);
				if (deviceGuardZone != null)
					TargetZones.Add(deviceGuardZone);
				else
				{
					var gkDeviceGuardZone = new GKDeviceGuardZone();
					gkDeviceGuardZone.GuardZone = guardZone;
					gkDeviceGuardZone.GuardZoneUID = guardZone.UID;
					SourceZones.Add(new DeviceGuardZoneViewModel(gkDeviceGuardZone, device));
				}
			}

			SelectedTargetZone = TargetZones.FirstOrDefault();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public SortableObservableCollection<DeviceGuardZoneViewModel> SourceZones { get; private set; }

		DeviceGuardZoneViewModel _selectedSourceZone;
		public DeviceGuardZoneViewModel SelectedSourceZone
		{
			get { return _selectedSourceZone; }
			set
			{
				_selectedSourceZone = value;
				OnPropertyChanged(() => SelectedSourceZone);
			}
		}

		public SortableObservableCollection<DeviceGuardZoneViewModel> TargetZones { get; private set; }

		DeviceGuardZoneViewModel _selectedTargetZone;
		public DeviceGuardZoneViewModel SelectedTargetZone
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
			var deviceGuardZoneViewModels = new List<DeviceGuardZoneViewModel>();
			foreach (var selectedZone in SelectedSourceZones)
			{
				var deviceGuardZoneViewModel = selectedZone as DeviceGuardZoneViewModel;
				if (deviceGuardZoneViewModel != null)
					deviceGuardZoneViewModels.Add(deviceGuardZoneViewModel);
			}
			foreach (var deviceGuardZoneViewModel in deviceGuardZoneViewModels)
			{
				TargetZones.Add(deviceGuardZoneViewModel);
				SourceZones.Remove(deviceGuardZoneViewModel);
			}
			TargetZones.Sort(x => x.No);
			SelectedTargetZone = TargetZones.LastOrDefault();
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
			var deviceGuardZoneViewModels = new List<DeviceGuardZoneViewModel>();
			foreach (var selectedZone in SelectedTargetZones)
			{
				var deviceGuardZoneViewModel = selectedZone as DeviceGuardZoneViewModel;
				if (deviceGuardZoneViewModel != null)
					deviceGuardZoneViewModels.Add(deviceGuardZoneViewModel);
			}
			foreach (var zoneViewModel in deviceGuardZoneViewModels)
			{
				SourceZones.Add(zoneViewModel);
				TargetZones.Remove(zoneViewModel);
			}
			SourceZones.Sort(x => x.No);
			SelectedSourceZone = SourceZones.LastOrDefault();
			OnPropertyChanged(() => TargetZones);

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
			TargetZones.Sort(x => x.No);
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
			SourceZones.Sort(x => x.No);
			TargetZones.Clear();
			SelectedSourceZone = SourceZones.FirstOrDefault();
		}

		public RelayCommand CreateNewCommand { get; private set; }
		void OnCreateNew()
		{
			var createZoneEventArg = new CreateGKGuardZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				var deviceGuardZone = new GKDeviceGuardZone();
				deviceGuardZone.GuardZone = createZoneEventArg.Zone;
				TargetZones.Add(new DeviceGuardZoneViewModel(deviceGuardZone, Device));
				if (TargetZones.Count == 1)
				{
					SaveCommand.Execute();
				}
			}
		}

		public bool CanAdd(object parameter)
		{
			return SelectedSourceZone != null;
		}

		public bool CanRemove(object parameter)
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
			DeviceGuardZones = new ObservableCollection<DeviceGuardZoneViewModel>(TargetZones);
			return base.Save();
		}
	}
}