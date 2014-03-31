//using System.Collections.Generic;
//using System.Linq;
//using XFiresecAPI;

//namespace FiresecClient
//{
//    public static partial class UpdateConfigurationHelper
//    {
//        public static void PrepareDescriptors(XDeviceConfiguration deviceConfiguration)
//        {
//            DeviceConfiguration = deviceConfiguration;
//            PrepareZones();
//            PrepareInputOutputDependences();
//            PrepareDirections();
//            PreparePumpStations();
//            PrepareMPTs();
//            PrepareDelays();
//        }

//        static void PrepareZones()
//        {
//            foreach (var zone in DeviceConfiguration.Zones)
//            {
//                zone.KauDatabaseParent = null;
//                zone.GkDatabaseParent = null;

//                var gkParents = new HashSet<XDevice>();
//                foreach (var device in zone.Devices)
//                {
//                    var gkParent = device.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                    gkParents.Add(gkParent);
//                }

//                var gkDevice = gkParents.FirstOrDefault();
//                if (gkDevice != null)
//                {
//                    zone.GkDatabaseParent = gkDevice;
//                }
//            }
//        }

//        static void PrepareInputOutputDependences()
//        {
//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                device.ClearDescriptor();
//            }
//            foreach (var zone in DeviceConfiguration.Zones)
//            {
//                zone.ClearDescriptor();
//            }
//            foreach (var direction in DeviceConfiguration.Directions)
//            {
//                direction.ClearDescriptor();
//            }
//            foreach (var pumpStation in DeviceConfiguration.PumpStations)
//            {
//                pumpStation.ClearDescriptor();
//            }
//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                mpt.ClearDescriptor();
//            }
//            foreach (var delay in DeviceConfiguration.Delays)
//            {
//                delay.ClearDescriptor();
//            }

//            foreach (var device in DeviceConfiguration.Devices)
//            {
//                LinkDeviceLogic(device, device.DeviceLogic.Clauses);
//                LinkDeviceLogic(device, device.DeviceLogic.OffClauses);
//            }

//            foreach (var zone in DeviceConfiguration.Zones)
//            {
//                LinkXBases(zone, zone);
//                foreach (var device in zone.Devices)
//                {
//                    LinkXBases(zone, device);
//                }
//            }

//            foreach (var direction in DeviceConfiguration.Directions)
//            {
//                foreach (var zone in direction.InputZones)
//                {
//                    LinkXBases(direction, zone);
//                }

//                foreach (var device in direction.InputDevices)
//                {
//                    LinkXBases(direction, device);
//                }
//            }

//            foreach (var pumpStation in DeviceConfiguration.PumpStations)
//            {
//                LinkDeviceLogic(pumpStation, pumpStation.StartLogic.Clauses);
//                LinkDeviceLogic(pumpStation, pumpStation.StopLogic.Clauses);
//                LinkDeviceLogic(pumpStation, pumpStation.AutomaticOffLogic.Clauses);
//            }

//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                LinkDeviceLogic(mpt, mpt.StartLogic.Clauses);
//            }

//            foreach (var delay in DeviceConfiguration.Delays)
//            {
//                LinkDeviceLogic(delay, delay.DeviceLogic.Clauses);
//            }
//        }

//        static void LinkDeviceLogic(XBase xBase, List<XClause> clauses)
//        {
//            if (clauses != null)
//            {
//                foreach (var clause in clauses)
//                {
//                    foreach (var clauseDevice in clause.Devices)
//                        LinkXBases(xBase, clauseDevice);
//                    foreach (var zone in clause.Zones)
//                        LinkXBases(xBase, zone);
//                    foreach (var direction in clause.Directions)
//                        LinkXBases(xBase, direction);
//                    foreach (var mpt in clause.MPTs)
//                        LinkXBases(xBase, mpt);
//                    foreach (var delay in clause.Delays)
//                        LinkXBases(xBase, delay);
//                }
//            }
//        }

//        static void PrepareDirections()
//        {
//            foreach (var direction in DeviceConfiguration.Directions)
//            {
//                direction.KauDatabaseParent = null;
//                direction.GkDatabaseParent = null;

//                var inputZone = direction.InputZones.FirstOrDefault();
//                if (inputZone != null)
//                {
//                    if (inputZone.GkDatabaseParent != null)
//                        direction.GkDatabaseParent = inputZone.GkDatabaseParent;
//                }

//                var inputDevice = direction.InputDevices.FirstOrDefault();
//                if (inputDevice != null)
//                {
//                    direction.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }

//                var outputDevice = direction.OutputDevices.FirstOrDefault();
//                if (outputDevice != null)
//                {
//                    direction.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }
//            }
//        }

//        static void PreparePumpStations()
//        {
//            foreach (var pumpStation in DeviceConfiguration.PumpStations)
//            {
//                pumpStation.KauDatabaseParent = null;
//                pumpStation.GkDatabaseParent = null;

//                var inputZone = pumpStation.ClauseInputZones.FirstOrDefault();
//                if (inputZone != null)
//                {
//                    if (inputZone.GkDatabaseParent != null)
//                        pumpStation.GkDatabaseParent = inputZone.GkDatabaseParent;
//                }

//                var inputDevice = pumpStation.ClauseInputDevices.FirstOrDefault();
//                if (inputDevice != null)
//                {
//                    pumpStation.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }

//                var outputDevice = pumpStation.NSDevices.FirstOrDefault();
//                if (outputDevice != null)
//                {
//                    pumpStation.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }
//            }
//        }

//        static void PrepareMPTs()
//        {
//            foreach (var mpt in DeviceConfiguration.MPTs)
//            {
//                mpt.KauDatabaseParent = null;
//                mpt.GkDatabaseParent = null;

//                var inputZone = mpt.ClauseInputZones.FirstOrDefault();
//                if (inputZone != null)
//                {
//                    if (inputZone.GkDatabaseParent != null)
//                        mpt.GkDatabaseParent = inputZone.GkDatabaseParent;
//                }

//                var inputDevice = mpt.ClauseInputDevices.FirstOrDefault();
//                if (inputDevice != null)
//                {
//                    mpt.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }

//                var outputDevice = mpt.Devices.FirstOrDefault();
//                if (outputDevice != null)
//                {
//                    mpt.GkDatabaseParent = outputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }
//            }
//        }

//        static void PrepareDelays()
//        {
//            foreach (var delay in DeviceConfiguration.Delays)
//            {
//                delay.KauDatabaseParent = null;
//                delay.GkDatabaseParent = null;

//                var inputDirection = delay.ClauseInputDirections.FirstOrDefault();
//                if (inputDirection != null)
//                {
//                    delay.GkDatabaseParent = inputDirection.GkDatabaseParent;
//                }

//                var inputZone = delay.ClauseInputZones.FirstOrDefault();
//                if (inputZone != null)
//                {
//                    if (inputZone.GkDatabaseParent != null)
//                        delay.GkDatabaseParent = inputZone.GkDatabaseParent;
//                }

//                var inputDevice = delay.ClauseInputDevices.FirstOrDefault();
//                if (inputDevice != null)
//                {
//                    delay.GkDatabaseParent = inputDevice.AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK);
//                }
//            }
//        }

//        public static void LinkXBases(XBase value, XBase dependsOn)
//        {
//            AddInputOutputObject(value.InputXBases, dependsOn);
//            AddInputOutputObject(dependsOn.OutputXBases, value);
//        }

//        static void AddInputOutputObject(List<XBase> objects, XBase newObject)
//        {
//            if (!objects.Contains(newObject))
//                objects.Add(newObject);
//        }
//    }
//}