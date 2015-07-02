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
		public ReflectionViewModel(GKDevice device)
		{
			switch (device.Driver.DriverType)
			{
				case GKDriverType.RSR2_GKMirrorFireZone:
					HasFireZones = true;
					break;

				case GKDriverType.RSR2_GKMirrorGuardZone:
					HasGuardZones = true;
					break;

				case GKDriverType.RSR2_GKMirrorPerformersDevice:
					HasMPT = true;
					HasDevices = true;
					HasDirections = true;
					HasNS = true;
					HasDelay = true;
					break;

				case GKDriverType.RSR2_GKMirrorFightFireZone:
					HasDirections = true;
					HasFireZones = true;
					break;

				case GKDriverType.RSR2_GKMirrorDirection:
					HasDirections = true;
					break;

				case GKDriverType.RSR2_GKMirrorDetectorsDevice:
					HasMPT = true;
					HasDevices = true;
					HasDirections = true;
					HasNS = true;
					HasDelay = true;
					break;
			}

			Title = "Выбор настройки отражения";
			Device = device;
			if (device.GKReflectionItem == null)
			device.GKReflectionItem = new GKReflectionItem();
			var zones = GKManager.Zones.Where(x => Device.GKReflectionItem.ZoneUIDs.Contains(x.UID)).ToList();
			ZonesSelectationViewModel = new ZonesSelectationViewModel(zones, true);
			var guardzones = GKManager.GuardZones.Where(x => Device.GKReflectionItem.GuardZoneUIDs.Contains(x.UID)).ToList();
			GuardZonesSelectationViewModel = new GuardZonesSelectationViewModel(guardzones);
			var directions = GKManager.Directions.Where(x => Device.GKReflectionItem.DiretionUIDs.Contains(x.UID)).ToList();
			DirectionsSelectationViewModel = new DirectionsSelectationViewModel(directions);
			var delays = GKManager.Delays.Where(x => Device.GKReflectionItem.DelayUIDs.Contains(x.UID)).ToList();
			DelaysSelectationViewModel = new DelaysSelectationViewModel(delays);
			var devices = GKManager.Devices.Where(x => Device.GKReflectionItem.DeviceUIDs.Contains(x.UID)).ToList();
			DevicesSelectationViewModel = new DevicesSelectationViewModel(devices);
			var ns = GKManager.PumpStations.Where(x => Device.GKReflectionItem.NSUIDs.Contains(x.UID)).ToList();
			PumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(ns);
			var mpts = GKManager.MPTs.Where(x => Device.GKReflectionItem.MPTUIDs.Contains(x.UID)).ToList();
			MPTsSelectationViewModel = new MPTsSelectationViewModel(mpts);
			
		}

		protected override bool Save()
		{
			Device.GKReflectionItem.ZoneUIDs = ZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();
			Device.GKReflectionItem.GuardZoneUIDs = GuardZonesSelectationViewModel.TargetZones.Select(x=> x.UID).ToList();
			Device.GKReflectionItem.DiretionUIDs = DirectionsSelectationViewModel.TargetDirections.Select(x => x.UID).ToList();
			Device.GKReflectionItem.DelayUIDs = DelaysSelectationViewModel.TargetDelays.Select(x => x.UID).ToList();
			Device.GKReflectionItem.DeviceUIDs = DevicesSelectationViewModel.Devices.Select(x => x.UID).ToList();
			Device.GKReflectionItem.NSUIDs = PumpStationsSelectationViewModel.TargetPumpStations.Select(x => x.UID).ToList();
			Device.GKReflectionItem.MPTUIDs = MPTsSelectationViewModel.TargetMPTs.Select(x => x.UID).ToList();
			return base.Save();
		}
	}
}
