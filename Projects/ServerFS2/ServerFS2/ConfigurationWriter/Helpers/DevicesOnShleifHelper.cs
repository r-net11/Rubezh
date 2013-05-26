using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public static class DevicesOnShleifHelper
	{
		public static List<DevicesOnShleif> GetLocalForZone(Device parentPanel, Zone zone)
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= parentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}

			var effectorDevices = GetDevicesInLogic(zone);
			foreach (var device in effectorDevices.OrderBy(x=>x.IntAddress))
			{
				if (device.ParentPanel.UID == parentPanel.UID)
				{
					var shleifNo = device.ShleifNo;
					if(device.Driver.DriverType == DriverType.PumpStation)
						shleifNo = 1;
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == shleifNo);
					if (devicesOnShleif != null)
					{
						devicesOnShleif.Devices.Add(device);
					}
				}
			}
			return devicesOnShleifs;
		}

		public static List<Device> GetRemoteForZone(Device parentPanel, Zone zone)
		{
			var devices = new HashSet<Device>();
			foreach (var device in GetDevicesInLogic(zone))
			{
				foreach (var clause in device.ZoneLogic.Clauses)
				{
					var allZonesAreRemote = true;
					foreach (var clauseZone in clause.Zones)
					{
						if (clauseZone.DevicesInZone.FirstOrDefault().ParentPanel.UID == device.ParentPanel.UID)
							allZonesAreRemote = false;
					}

					if (clause.Operation.Value == ZoneLogicOperation.Any || allZonesAreRemote)
					{
						foreach (var clauseZone in clause.Zones)
						{
							if (clauseZone.UID == zone.UID)
							{
								if (device.ParentPanel.UID != parentPanel.UID)
								{
									devices.Add(device);
								}
							}
						}
					}
				}
			}
			return devices.ToList();
		}

        public static List<Device> GetRemotePanelsForZone(Device parentPanel, Zone zone)
        {
            var devices = new HashSet<Device>();
            foreach (var binaryPanel in BinaryConfigurationHelper.Current.BinaryPanels)
            {
                if (binaryPanel.ParentPanel != parentPanel)
                {
                    if (binaryPanel.TempRemoteZones.Contains(zone))
                        devices.Add(binaryPanel.ParentPanel);
                }
            }
            return devices.ToList();
        }

		public static List<DevicesOnShleif> GetLocalForPanel(Device parentPanel)
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= parentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in parentPanel.GetRealChildren())
			{
				if (device.ParentPanel.UID == parentPanel.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					if (devicesOnShleif != null)
					{
						devicesOnShleif.Devices.Add(device);
					}
				}
			}
			return devicesOnShleifs;
		}

		public static List<DevicesOnShleif> GetLocalForPanelToMax(Device parentPanel)
		{
			var devicesOnShleifs = GetLocalForPanel(parentPanel);

			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var maxAddress = 0;
				if (devicesOnShleif.Devices.Count > 0)
					maxAddress = devicesOnShleif.Devices.Max(x => x.AddressOnShleif);

				var devices = new List<Device>();
				for (int i = 1; i <= maxAddress; i++)
				{
					devices.Add(null);
				}
				foreach (var device in devicesOnShleif.Devices)
				{
					devices[device.AddressOnShleif - 1] = device;
				}
				devicesOnShleif.Devices = devices;
			}

			return devicesOnShleifs;
		}

		static List<Device> GetDevicesInLogic(Zone zone)
		{
			var result = zone.DevicesInZoneLogic;
			foreach (var device in zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
				{
					result.Add(device);
				}
			}
			var hashSet = new HashSet<Device>(result);
			return hashSet.ToList();
		}
	}
}

//DevicesOnShleifHelper
//diff --git a/Projects/ServerFS2/ServerFS2/ConfigurationWriter/Helpers/DevicesOnShleifHelper.cs b/Projects/ServerFS2/ServerFS2/ConfigurationWriter/Helpers/DevicesOnShleifHelper.cs
//index 07508aa..f14aa99 100644
//-- a/Projects/ServerFS2/ServerFS2/ConfigurationWriter/Helpers/DevicesOnShleifHelper.cs
//++ b/Projects/ServerFS2/ServerFS2/ConfigurationWriter/Helpers/DevicesOnShleifHelper.cs
//@@ -38,20 +38,98 @@ namespace ServerFS2.ConfigurationWriter
//            return devicesOnShleifs;
//        }

//        public static List<Device> GetRemoteForZone(Device parentPanel, Zone zone)
//        public static List<Device> GetRemoteDevicesForZone2(Device parentPanel, Zone zone)
//        {
//            if (!zone.DevicesInZone.Any(x => x.ParentPanel.UID == parentPanel.UID))
//                return new List<Device>();

//            var devices = new HashSet<Device>();
//            foreach (var deviceInLogic in zone.DevicesInZoneLogic)
//            {
//                if (deviceInLogic.ParentPanel.UID == parentPanel.UID)
//                    continue;

//                foreach (var clause in deviceInLogic.ZoneLogic.Clauses)
//                {
//                    var hasLocalZones = false;
//                    foreach (var clauseZone in clause.Zones)
//                    {
//                        foreach (var deviceInZone in clauseZone.DevicesInZone)
//                        {
//                            if (deviceInZone.ParentPanel.UID == deviceInLogic.ParentPanel.UID)
//                                hasLocalZones = true;
//                        }
//                    }

//                    foreach (var clauseZone in clause.Zones)
//                    {
//                        if (clauseZone.UID == zone.UID)
//                        {
//                            //if (clause.Operation.Value == ZoneLogicOperation.All && hasLocalZones)
//                            if (hasLocalZones)
//                            {
//                                devices.Add(deviceInLogic);
//                            }
//                        }
//                    }
//                }
//            }
//            if (devices.Count == 0)
//            {
//                foreach (var deviceInLogic in zone.DevicesInZoneLogic)
//                {
//                    if (deviceInLogic.ParentPanel.UID == parentPanel.UID)
//                        continue;

//                    foreach (var clause in deviceInLogic.ZoneLogic.Clauses)
//                    {
//                        foreach (var clauseZone in clause.Zones)
//                        {
//                            if (clauseZone.UID == zone.UID)
//                            {
//                                if (clause.Operation.Value == ZoneLogicOperation.All)
//                                {
//                                    devices.Add(deviceInLogic);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return devices.ToList();
//        }

//        public static List<Device> GetRemoteDevicesForZone(Device parentPanel, Zone zone)
//        {
//            if (!zone.DevicesInZone.Any(x => x.ParentPanel.UID == parentPanel.UID))
//                return new List<Device>();

//            var devices = new HashSet<Device>();
//            foreach (var device in GetDevicesInLogic(zone))
//            var devicesInZone = GetDevicesInLogic(zone);
//            foreach (var device in devicesInZone)
//            {
//                //var localBinaryPanel = BinaryConfigurationHelper.Current.BinaryPanels.FirstOrDefault(x => x.ParentPanel == device.ParentPanel);
//                //if (localBinaryPanel.TempRemoteZones.Contains(zone))
//                //{
//                //    if (device.ParentPanel.UID != parentPanel.UID)
//                //        devices.Add(device);
//                //}

//                foreach (var clause in device.ZoneLogic.Clauses)
//                {
//                    var allZonesAreRemote = true;
//                    var hasLocalZones = false;
//                    foreach (var clauseZone in clause.Zones)
//                    {
//                        if (clauseZone.DevicesInZone.FirstOrDefault().ParentPanel.UID == device.ParentPanel.UID)
//                            allZonesAreRemote = false;
//                        foreach (var deviceInZone in clauseZone.DevicesInZone)
//                        {
//                            if (deviceInZone.ParentPanel.UID == device.ParentPanel.UID)
//                                hasLocalZones = true;
//                        }
//                    }
//                    if (clause.Operation.Value == ZoneLogicOperation.Any || allZonesAreRemote)

//                    //if (clause.Operation.Value == ZoneLogicOperation.Any && hasLocalZones)
//                    if (clause.Operation.Value == ZoneLogicOperation.All && hasLocalZones)
//                    //if (clause.Operation.Value == ZoneLogicOperation.All)
//                    {
//                        foreach (var clauseZone in clause.Zones)
//                        {
//@@ -66,21 +144,95 @@ namespace ServerFS2.ConfigurationWriter
//                    }
//                }
//            }

//            //var result = new HashSet<Device>();
//            //foreach (var device in devices)
//            //{
//            //    foreach (var clause in device.ZoneLogic.Clauses)
//            //    {
//            //        var hasLocalZones = false;
//            //        foreach (var clauseZone in clause.Zones)
//            //        {
//            //            foreach (var deviceInZone in clauseZone.DevicesInZone)
//            //            {
//            //                if(deviceInZone.ParentPanel.UID == device.ParentPanel.UID)
//            //                    hasLocalZones = true;
//            //            }
//            //        }

//            //        if (clause.Operation.Value == ZoneLogicOperation.All && hasLocalZones)
//            //        {
//            //            result.Add(device);
//            //            //foreach (var clauseZone in clause.Zones)
//            //            //{
//            //            //    if (clauseZone.UID == zone.UID)
//            //            //    {
//            //            //        if (device.ParentPanel.UID != parentPanel.UID)
//            //            //        {
//            //            //            result.Add(device);
//            //            //        }
//            //            //    }
//            //            //}
//            //        }
//            //    }
//            //}
//            //return result.ToList();

//            return devices.ToList();
//        }

//        public static List<Device> GetRemotePanelsForZone2(Device parentPanel, Zone zone)
//        {
//            var devices = new HashSet<Device>();
//            foreach (var device in GetRemoteDevicesForZone(parentPanel, zone))
//            {
//                devices.Add(device.ParentPanel);
//            }
//            return devices.ToList();
//        }

//        public static List<Device> GetRemotePanelsForZone(Device parentPanel, Zone zone)
//        {
//           var devices = new HashSet<Device>();
//           foreach (var binaryPanel in BinaryConfigurationHelper.Current.BinaryPanels)
//           {
//               if (binaryPanel.ParentPanel != parentPanel)
//               {
//                   if (binaryPanel.TempRemoteZones.Contains(zone))
//                       devices.Add(binaryPanel.ParentPanel);
//               }
//           }
//           return devices.ToList();
//            //if (!zone.DevicesInZone.Any(x => x.ParentPanel.UID == parentPanel.UID))
//            //    return new List<Device>();

//            var devices = new HashSet<Device>();
//            foreach (var binaryPanel in BinaryConfigurationHelper.Current.BinaryPanels)
//            {
//                if(binaryPanel.ParentPanel.UID != parentPanel.UID)
//                {
//                    if (binaryPanel.TempRemoteZones.Contains(zone))
//                    {
//                        devices.Add(binaryPanel.ParentPanel);
//                    }
//                    //if (binaryPanel.TempLocalZones.Contains(zone))
//                    //{
//                    //    devices.Add(binaryPanel.ParentPanel);
//                    //}
//                }
//            }
//            return devices.ToList();

//            //var remoteDevices = GetRemoteDevicesForZone(parentPanel, zone);
//            //foreach (var device in remoteDevices)
//            //{
//            //    if (device.ParentPanel.UID != parentPanel.UID)
//            //        devices.Add(device);
//            //}
//            //return devices.ToList();

//            //if (!zone.DevicesInZone.Any(x => x.ParentPanel.UID == parentPanel.UID))
//            //    return new List<Device>();

//            //foreach (var binaryPanel in BinaryConfigurationHelper.Current.BinaryPanels)
//            //{
//            //    if (binaryPanel.ParentPanel != parentPanel)
//            //    {
//            //        if (binaryPanel.TempRemoteZones.Contains(zone))
//            //            devices.Add(binaryPanel.ParentPanel);
//            //    }
//            //}
//            //return devices.ToList();
//        }

//        public static List<DevicesOnShleif> GetLocalForPanel(Device parentPanel)
