using RubezhClient;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI;

namespace GKModule.ViewModels
{
	class OPCDetailsViewModel : SaveCancelDialogViewModel
	{
		public OPCDetailsViewModel()
		{		
			var zones = GKManager.Zones.Where(x => GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs.Contains(x.UID)).ToList();
			ZonesSelectationViewModel = new ZonesSelectationViewModel(zones, true);
			var guardzones = GKManager.GuardZones.Where(x => GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs.Contains(x.UID)).ToList();
			GuardZonesSelectationViewModel = new GuardZonesSelectationViewModel(guardzones);
			var directions = GKManager.Directions.Where(x => GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs.Contains(x.UID)).ToList();
			DirectionsSelectationViewModel = new DirectionsSelectationViewModel(directions);
			var delays = GKManager.Delays.Where(x => GKManager.DeviceConfiguration.OPCSettings.DelayUIDs.Contains(x.UID)).ToList();
			DelaysSelectationViewModel = new DelaysSelectationViewModel(delays);
			var devices = GKManager.Devices.Where(x => GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs.Contains(x.UID)).ToList();
			DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x => x.Driver.IsReal).ToList());
			var mpts = GKManager.MPTs.Where(x => GKManager.DeviceConfiguration.OPCSettings.MPTUIDs.Contains(x.UID)).ToList();
			MPTsSelectationViewModel = new MPTsSelectationViewModel(mpts);
			var ns = GKManager.PumpStations.Where(x => GKManager.DeviceConfiguration.OPCSettings.NSUIDs.Contains(x.UID)).ToList();
			PumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(ns);
			var doors = GKManager.Doors.Where(x => GKManager.DeviceConfiguration.OPCSettings.DoorUIDs.Contains(x.UID)).ToList();
			DoorsSelectationViewModel = new DoorsSelectationViewModel(doors);
		}

		public ZonesSelectationViewModel ZonesSelectationViewModel { get; private set; }

		public GuardZonesSelectationViewModel GuardZonesSelectationViewModel { get; private set; }

		public DirectionsSelectationViewModel DirectionsSelectationViewModel { get; private set; }

		public DelaysSelectationViewModel DelaysSelectationViewModel { get; private set; }

		public MPTsSelectationViewModel MPTsSelectationViewModel { get; private set; }

		public PumpStationsSelectationViewModel PumpStationsSelectationViewModel { get; private set; }

		public DevicesSelectationViewModel DevicesSelectationViewModel { get; private set; }

		public DoorsSelectationViewModel DoorsSelectationViewModel { get; private set; }

		protected override bool Save()
		{

		GKManager.DeviceConfiguration.OPCSettings.ZoneUIDs = ZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.GuardZoneUIDs = GuardZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.DelayUIDs = DelaysSelectationViewModel.TargetDelays.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.DeviceUIDs = DevicesSelectationViewModel.Devices.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.DiretionUIDs = DirectionsSelectationViewModel.TargetDirections.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.MPTUIDs = MPTsSelectationViewModel.TargetMPTs.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.NSUIDs = PumpStationsSelectationViewModel.TargetPumpStations.Select(x => x.UID).ToList();

		GKManager.DeviceConfiguration.OPCSettings.DoorUIDs = DoorsSelectationViewModel.TargetDoors.Select(x => x.UID).ToList();

			return base.Save();
		}
	}
}
