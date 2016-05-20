using Common;
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
			if (device.Driver.DriverType == GKDriverType.DetectorDevicesMirror)
			{
				var devices = GKManager.Devices.Where(x => Device.GKReflectionItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x=> x.Driver.HasZone).ToList());
			}
			if (device.Driver.DriverType == GKDriverType.ControlDevicesMirror)
			{
				var devices = GKManager.Devices.Where(x => Device.GKReflectionItem.DeviceUIDs.Contains(x.UID)).ToList();
				DevicesSelectationViewModel = new DevicesSelectationViewModel(devices, GKManager.Devices.Where(x => x.Driver.IsControlDevice).ToList());
			}
			var ns = GKManager.PumpStations.Where(x => Device.GKReflectionItem.NSUIDs.Contains(x.UID)).ToList();
			PumpStationsSelectationViewModel = new PumpStationsSelectationViewModel(ns);
			var mpts = GKManager.MPTs.Where(x => Device.GKReflectionItem.MPTUIDs.Contains(x.UID)).ToList();
			MPTsSelectationViewModel = new MPTsSelectationViewModel(mpts);

			switch (device.Driver.DriverType)
			{
				case GKDriverType.FireZonesMirror:
					HasFireZones = true;
					IsFireZones = true;
					break;

				case GKDriverType.GuardZonesMirror:
					HasGuardZones = true;
					IsGuardZones = true;
					break;

				case GKDriverType.ControlDevicesMirror:
					HasMPT = true;
					HasDevices = true;
					HasDirections = true;
					HasNS = true;
					HasDelay = true;
					IsDevices = DevicesSelectationViewModel.DevicesList.Any();
					IsDirections = !IsDevices && DirectionsSelectationViewModel.TargetDirections.Any();
					IsDelay =!IsDirections &&!IsDevices && DelaysSelectationViewModel.TargetDelays.Any();
					IsNS = !IsDirections && !IsDevices && !IsDelay && PumpStationsSelectationViewModel.TargetPumpStations.Any();
					IsDevices = !(IsMPT = !IsDirections && !IsDevices && !IsDelay &&!IsNS && MPTsSelectationViewModel.TargetMPTs.Any());
					break;

				case GKDriverType.FirefightingZonesMirror:
					HasDirections = true;
					HasFireZones = true;
					IsFireZones = ZonesSelectationViewModel.TargetZones.Any() ? true :  DirectionsSelectationViewModel.TargetDirections.Any() ? !(IsDirections = true) : true ;
					break;

				case GKDriverType.DirectionsMirror:
					HasDirections = true;
					IsDirections = true;
					break;

				case GKDriverType.DetectorDevicesMirror:
					HasDevices = true;
					IsDevices = true;
					break;
			}
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
			GKManager.ClearMirror(Device);
			ZonesSelectationViewModel.TargetZones.ForEach(x => GKManager.AddToMirror(x, Device));
			GuardZonesSelectationViewModel.TargetZones.ForEach(x => GKManager.AddToMirror(x, Device));
			DirectionsSelectationViewModel.TargetDirections.ForEach(x => GKManager.AddToMirror(x, Device));
			DelaysSelectationViewModel.TargetDelays.ForEach(x => GKManager.AddToMirror(x, Device));
			if (DevicesSelectationViewModel != null)
				DevicesSelectationViewModel.Devices.ForEach(x => GKManager.AddToMirror(x, Device));
			PumpStationsSelectationViewModel.TargetPumpStations.ForEach(x => GKManager.AddToMirror(x, Device));
			MPTsSelectationViewModel.TargetMPTs.ForEach(x => GKManager.AddToMirror(x, Device));
			return base.Save();
		}
	}
}
