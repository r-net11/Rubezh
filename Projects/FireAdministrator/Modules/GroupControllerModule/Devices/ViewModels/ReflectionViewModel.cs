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

			Title = "Выбор зон";
			Device = device;
			if (device.GKReflectionItem == null)
			device.GKReflectionItem = new GKReflectionItem();
			var zones = GKManager.Zones.Where(x => Device.GKReflectionItem.ZoneUIDs.Contains(x.UID)).ToList();
			ZonesSelectationViewModel = new ZonesSelectationViewModel(zones, true);
			var guardzones = GKManager.GuardZones.Where(x => Device.GKReflectionItem.GuardZoneUIDs.Contains(x.UID)).ToList();
			GuardZonesSelectationViewModel = new GuardZonesSelectationViewModel(guardzones);
			//DirectionsSelectationViewModel = new DirectionsSelectationViewModel(device.Directions);
			//DelaysSelectationViewModel = new DelaysSelectationViewModel(device.);	
		}

		protected override bool Save()
		{
			Device.GKReflectionItem.ZoneUIDs = ZonesSelectationViewModel.TargetZones.Select(x => x.UID).ToList();
			return base.Save();
		}
	}
}
