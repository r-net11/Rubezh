using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceStates { get; set; }
        public static LibraryConfiguration LibraryConfiguration { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static PlansConfiguration PlansConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }

        [Obsolete("Use GetConfiguration")]
        public static void SelectiveFetch(bool updateFiles = true)
        {
            GetConfiguration(updateFiles);
        }

        public static void GetConfiguration(bool updateFiles = true)
        {
            if (updateFiles)
                FileHelper.Synchronize();

            SystemConfiguration = FiresecService.GetSystemConfiguration();
            LibraryConfiguration = FiresecService.GetLibraryConfiguration();
            PlansConfiguration = FiresecService.GetPlansConfiguration();
            SecurityConfiguration = FiresecService.GetSecurityConfiguration();
            Drivers = FiresecService.GetDrivers();
            DeviceConfiguration = FiresecService.GetDeviceConfiguration();
            DeviceStates = FiresecService.GetStates();

            UpdateDrivers();
            UpdateConfiguration();
            InvalidateConfiguration();
            UpdatePlansConfiguration();
        }

        public static void UpdateDrivers()
        {
            if (Drivers != null)
            {
                Drivers.ForEach(x => x.ImageSource = FileHelper.GetIconFilePath(x.ImageSource) + ".ico");
            }
        }

        public static void UpdateConfiguration()
        {
            PlansConfiguration.Update();
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver.IsIndicatorDevice || device.IndicatorLogic != null)
                    device.IndicatorLogic.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.DeviceUID != Guid.Empty))
                    {
                        clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                    }
                }
            }
        }

        public static void UpdatePlansConfiguration()
        {
            FiresecManager.DeviceConfiguration.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

            foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
            {
                for (int i = plan.ElementDevices.Count(); i > 0; i--)
                {
                    var elementDevice = plan.ElementDevices[i - 1];

                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                    if (device != null)
                    {
                        device.PlanElementUIDs.Add(elementDevice.UID);
                        elementDevice.Device = device;
                    }
                    else
                    {
                        plan.ElementDevices.RemoveAt(i - 1);
                    }
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                    if (deviceState != null)
                    {
                        elementDevice.DeviceState = deviceState;
                    }
                }

                foreach (var elementZone in plan.ElementPolygonZones)
                {
                    if (elementZone.ZoneNo.HasValue)
                    {
                        elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo.Value);
                    }
                }

                foreach (var elementZone in plan.ElementRectangleZones)
                {
                    if (elementZone.ZoneNo.HasValue)
                    {
                        elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo.Value);
                    }
                }

                foreach (var elementSubPlan in plan.ElementSubPlans)
                {
                    elementSubPlan.Plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == elementSubPlan.PlanUID);
                }
            }
        }

        public static void InvalidateConfiguration()
        {
            foreach (var device in DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == DriverType.Indicator)
                {
                    if (DeviceConfiguration.Devices.Any(x => x.UID == device.IndicatorLogic.DeviceUID) == false)
                    {
                        device.IndicatorLogic.DeviceUID = Guid.Empty;
                        device.IndicatorLogic.Device = null;
                    }

                    var zones = new List<ulong>();
                    foreach (var zoneNo in device.IndicatorLogic.Zones)
                    {
                        if (DeviceConfiguration.Zones.Any(x => x.No == zoneNo))
                            zones.Add(zoneNo);
                    }
                    device.IndicatorLogic.Zones = zones;
                }
                if (device.Driver.DriverType == DriverType.Direction)
                {
                    foreach (var pduGroupDevice in device.PDUGroupLogic.Devices)
                    {
                        if (DeviceConfiguration.Devices.Any(x => x.UID == pduGroupDevice.DeviceUID) == false)
                            pduGroupDevice.DeviceUID = Guid.Empty;
                    }
                }
                if (device.Driver.IsZoneDevice)
                {
                    if (DeviceConfiguration.Zones.Any(x => x.No == device.ZoneNo) == false)
                        device.ZoneNo = null;
                }
                if (device.Driver.IsZoneLogicDevice)
                {
                    if (device.ZoneLogic != null)
                    {
                        var clauses = new List<Clause>();
                        foreach (var clause in device.ZoneLogic.Clauses)
                        {
                            if (DeviceConfiguration.Devices.Any(x => x.UID == clause.DeviceUID) == false)
                            {
                                clause.DeviceUID = Guid.Empty;
                                clause.Device = null;
                            }

                            var zones = new List<ulong>();
                            foreach (var zoneNo in clause.Zones)
                            {
                                if (DeviceConfiguration.Zones.Any(x => x.No == zoneNo))
                                    zones.Add(zoneNo);
                            }
                            clause.Zones = zones;

                            if ((clause.Device != null) || (clause.Zones.Count > 0))
                                clauses.Add(clause);
                        }
                        device.ZoneLogic.Clauses = clauses;
                    }
                }

                if (device.Driver.DriverType != DriverType.Indicator)
                    device.IndicatorLogic = null;
                if (device.Driver.DriverType != DriverType.Direction)
                    device.PDUGroupLogic = null;
                if (!device.Driver.IsZoneDevice)
                    device.ZoneNo = null;
                if (!device.Driver.IsZoneLogicDevice)
                    device.ZoneLogic = null;
            }
        }

        public static List<Zone> GetChannelZones(Device device)
        {
            UpdateZoneDevices();

            var zones = new List<Zone>();

            var channelDevice = device.Channel;

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                if (channelDevice != null)
                {
                    if (zone.DevicesInZone.All(x => ((x.Channel != null) && (x.Channel.UID == channelDevice.UID))))
                        zones.Add(zone);
                }
                else
                {
                    if (zone.DevicesInZone.All(x => (x.Parent.UID == device.UID)))
                        zones.Add(zone);
                }
            }

            return zones;
        }

        public static void UpdateZoneDevices()
        {
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                zone.DevicesInZone = new List<Device>();
                zone.DeviceInZoneLogic = new List<Device>();
            }

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.Driver == null)
                {
                    System.Windows.MessageBox.Show("У устройства отсутствует драйвер");
                    continue;
                }

                if ((device.Driver.IsZoneDevice) && (device.ZoneNo != null))
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                    if (zone != null)
                    {
                        zone.DevicesInZone.Add(device);
                    }
                }
                if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
                {
                    foreach (var clause in device.ZoneLogic.Clauses)
                    {
                        foreach (var clauseZone in clause.Zones)
                        {
                            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == clauseZone);
                            if (zone != null)
                            {
                                zone.DeviceInZoneLogic.Add(device);
                            }
                        }
                    }
                }
            }
        }
    }
}