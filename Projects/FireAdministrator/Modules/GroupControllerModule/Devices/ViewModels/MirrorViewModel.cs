using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace GKModule.ViewModels 
{
	class MirrorViewModel:SaveCancelDialogViewModel
	{
		public MirrorViewModel(GKDevice device)
		{
			switch (device.Driver.DriverType)
			{
				case GKDriverType.RSR2_GKMirrorFireZone:
					HasFireZones = true;
					IsFireZones = true;
					break;

				case GKDriverType.RSR2_GKMirrorGuardZone:
					HasGuardZones = true;
					IsGuardZones = true;
					break;

				case GKDriverType.RSR2_GKMirrorPerformersDevice:
					HasMPT = true;
					HasDevices = true;
					HasDirections = true;
					HasNS = true;
					HasDelay = true;
					IsDirections = true;
					break;

				case GKDriverType.RSR2_GKMirrorFightFireZone:
					HasDirections = true;
					HasFireZones = true;
					IsDirections = true;
					break;

				case GKDriverType.RSR2_GKMirrorDirection:
					HasDirections = true;
					IsDirections = true;
					break;

				case GKDriverType.RSR2_GKMirrorDetectorsDevice:
					HasDevices = true;
					IsDevices = true;
					break;
			}

			Title = "Выбор настройки отражения";
			Device = device;
			var zones = GKManager.Zones.Where(x => Device.GKMirrorItem.ZoneUIDs.Contains(x.UID)).ToList();
			ZonesSelectationViewModel = new ZonesSelectationViewModel(zones, true);
			var guardzones = GKManager.GuardZones.Where(x => Device.GKMirrorItem.GuardZoneUIDs.Contains(x.UID)).ToList();
			GuardZonesSelectationViewModel = new GuardZonesSelectationViewModel(guardzones);
			var directions = GKManager.Directions.Where(x => Device.GKMirrorItem.DiretionUIDs.Contains(x.UID)).ToList();
			DirectionsSelectationViewModel = new DirectionsSelectationViewModel(directions);
			var delays = GKManager.Delays.Where(x => Device.GKMirrorItem.DelayUIDs.Contains(x.UID)).ToList();
			DelaysSelectationViewModel = new DelaysSelectationViewModel(delays);
			if (device.Driver.DriverType == GKDriverType.RSR2_GKMirrorDetectorsDevice)
			{
				var devices = GKManager.Devices.Where(x => Device.GKMirrorItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x=> x.Driver.HasZone).ToList());
			}
			if (device.Driver.DriverType == GKDriverType.RSR2_GKMirrorPerformersDevice)
			{
				var devices = GKManager.Devices.Where(x => Device.GKMirrorItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x => x.Driver.IsControlDevice).ToList());
			}
			var ns = GKManager.PumpStations.Where(x => Device.GKMirrorItem.NSUIDs.Contains(x.UID)).ToList();
			PumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(ns);
			var mpts = GKManager.MPTs.Where(x => Device.GKMirrorItem.MPTUIDs.Contains(x.UID)).ToList();
			MPTsSelectationViewModel = new MPTsSelectationViewModel(mpts);
		}

		public ZonesSelectationViewModel ZonesSelectationViewModel { get; private set; }

		public GuardZonesSelectationViewModel GuardZonesSelectationViewModel { get; private set; }

		public DirectionsSelectationViewModel DirectionsSelectationViewModel { get; private set; }

		public DelaysSelectationViewModel DelaysSelectationViewModel { get; private set; }

		public MPTsSelectationViewModel  MPTsSelectationViewModel { get; private set; }

		public PumpStationsSelectationViewModel PumpStationsSelectationViewModel { get; private set; }

		public DevicesSelectationViewModel DevicesSelectationViewModel { get; private set; }

		GKDevice Device { get; set; }

		public bool HasFireZones { get; private set; }

		public bool HasGuardZones { get; private set; }

		public bool HasMPT { get; private set; }

		public bool HasNS { get; private set; }

		public bool HasDelay { get; private set; }

		public bool HasDevices { get; private set; }

		public bool HasDirections { get; private set; }

		public bool IsFireZones { get; private set; }

		public bool IsGuardZones { get; private set; }

		public bool IsMPT { get; private set; }

		public bool IsNS { get; private set; }

		public bool IsDelay { get; private set; }

		public bool IsDevices { get; private set; }

		public bool IsDirections { get; private set; }

		protected override bool Save()
		{
			Device.GKMirrorItem.ZoneUIDs = ZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();

			Device.GKMirrorItem.Zones = ZonesSelectationViewModel.TargetZones.ToList();

			Device.GKMirrorItem.GuardZoneUIDs = GuardZonesSelectationViewModel.TargetZones.Select(x=> x.UID).ToList();

			Device.GKMirrorItem.GuardZones = GuardZonesSelectationViewModel.TargetZones.ToList();

			Device.GKMirrorItem.DiretionUIDs = DirectionsSelectationViewModel.TargetDirections.Select(x => x.UID).ToList();

			Device.GKMirrorItem.Diretions = DirectionsSelectationViewModel.TargetDirections.ToList();

			Device.GKMirrorItem.DelayUIDs = DelaysSelectationViewModel.TargetDelays.Select(x => x.UID).ToList();

			Device.GKMirrorItem.Delays = DelaysSelectationViewModel.TargetDelays.ToList();

			if (DevicesSelectationViewModel != null)
			{
				Device.GKMirrorItem.DeviceUIDs = DevicesSelectationViewModel.Devices.Select(x => x.UID).ToList();

				Device.GKMirrorItem.Devices = DevicesSelectationViewModel.Devices.ToList();
			}
			Device.GKMirrorItem.NSUIDs = PumpStationsSelectationViewModel.TargetPumpStations.Select(x => x.UID).ToList();

			Device.GKMirrorItem.NSs = PumpStationsSelectationViewModel.TargetPumpStations.ToList();

			Device.GKMirrorItem.MPTUIDs = MPTsSelectationViewModel.TargetMPTs.Select(x => x.UID).ToList();

			Device.GKMirrorItem.MPTs = MPTsSelectationViewModel.TargetMPTs.ToList();

			return base.Save();
			
		}

	}
}
