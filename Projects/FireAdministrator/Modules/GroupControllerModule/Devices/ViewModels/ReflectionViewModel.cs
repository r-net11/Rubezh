using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.GK;
using FiresecClient;

namespace GKModule.ViewModels 
{
	class ReflectionViewModel:SaveCancelDialogViewModel
	{
		public ReflectionViewModel(GKDevice device)
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
			var zones = GKManager.Zones.Where(x => Device.GKReflectionItem.ZoneUIDs.Contains(x.UID)).ToList();
			ZonesSelectationViewModel = new ZonesSelectationViewModel(zones, true);
			var guardzones = GKManager.GuardZones.Where(x => Device.GKReflectionItem.GuardZoneUIDs.Contains(x.UID)).ToList();
			GuardZonesSelectationViewModel = new GuardZonesSelectationViewModel(guardzones);
			var directions = GKManager.Directions.Where(x => Device.GKReflectionItem.DiretionUIDs.Contains(x.UID)).ToList();
			DirectionsSelectationViewModel = new DirectionsSelectationViewModel(directions);
			var delays = GKManager.Delays.Where(x => Device.GKReflectionItem.DelayUIDs.Contains(x.UID)).ToList();
			DelaysSelectationViewModel = new DelaysSelectationViewModel(delays);
			if (device.Driver.DriverType == GKDriverType.RSR2_GKMirrorDetectorsDevice)
			{
				var devices = GKManager.Devices.Where(x => Device.GKReflectionItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x=> x.Driver.HasZone).ToList());
			}
			if (device.Driver.DriverType == GKDriverType.RSR2_GKMirrorPerformersDevice)
			{
				var devices = GKManager.Devices.Where(x => Device.GKReflectionItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x => x.Driver.IsControlDevice).ToList());
			}
			var ns = GKManager.PumpStations.Where(x => Device.GKReflectionItem.NSUIDs.Contains(x.UID)).ToList();
			PumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(ns);
			var mpts = GKManager.MPTs.Where(x => Device.GKReflectionItem.MPTUIDs.Contains(x.UID)).ToList();
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
			Device.GKReflectionItem.ZoneUIDs = ZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();

			Device.GKReflectionItem.Zones = ZonesSelectationViewModel.TargetZones.ToList();

			Device.GKReflectionItem.GuardZoneUIDs = GuardZonesSelectationViewModel.TargetZones.Select(x=> x.UID).ToList();

			Device.GKReflectionItem.GuardZones = GuardZonesSelectationViewModel.TargetZones.ToList();

			Device.GKReflectionItem.DiretionUIDs = DirectionsSelectationViewModel.TargetDirections.Select(x => x.UID).ToList();

			Device.GKReflectionItem.Diretions = DirectionsSelectationViewModel.TargetDirections.ToList();

			Device.GKReflectionItem.DelayUIDs = DelaysSelectationViewModel.TargetDelays.Select(x => x.UID).ToList();

			Device.GKReflectionItem.Delays = DelaysSelectationViewModel.TargetDelays.ToList();

			if (DevicesSelectationViewModel != null)
			{
				Device.GKReflectionItem.DeviceUIDs = DevicesSelectationViewModel.Devices.Select(x => x.UID).ToList();

				Device.GKReflectionItem.Devices = DevicesSelectationViewModel.Devices.ToList();
			}
			Device.GKReflectionItem.NSUIDs = PumpStationsSelectationViewModel.TargetPumpStations.Select(x => x.UID).ToList();

			Device.GKReflectionItem.NSs = PumpStationsSelectationViewModel.TargetPumpStations.ToList();

			Device.GKReflectionItem.MPTUIDs = MPTsSelectationViewModel.TargetMPTs.Select(x => x.UID).ToList();

			Device.GKReflectionItem.MPTs = MPTsSelectationViewModel.TargetMPTs.ToList();

			return base.Save();
			
		}

	}
}
