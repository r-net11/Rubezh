using System;
using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InitializeLogic();
			InitializeDirections();
			UpdateGKChildrenDescription();
		}

		static void ClearAllReferences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Zones = new List<XZone>();
				device.Directions = new List<XDirection>();
                device.DevicesInLogic = new List<XDevice>();
			}
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.Devices = new List<XDevice>();
				zone.Directions = new List<XDirection>();
                zone.DevicesInLogic = new List<XDevice>();
			}
			foreach (var direction in DeviceConfiguration.Directions)
			{
				direction.InputZones = new List<XZone>();
				direction.InputDevices = new List<XDevice>();
                direction.OutputDevices = new List<XDevice>();
			}
		}

		static void InitializeDevicesInZone()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				var zoneUIDs = new List<Guid>();
				if (device.ZoneUIDs != null)
					foreach (var zoneUID in device.ZoneUIDs)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							zoneUIDs.Add(zoneUID);
							device.Zones.Add(zone);
							zone.Devices.Add(device);
						}
					}
				device.ZoneUIDs = zoneUIDs;
			}
		}

		static void InitializeLogic()
		{
			foreach (var logicDevice in DeviceConfiguration.Devices)
			{
				InvalidateOneLogic(logicDevice);
			}
		}

		public static void InvalidateOneLogic(XDevice device)
		{
			var clauses = new List<XClause>();
			if (device.DeviceLogic.Clauses != null)
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					clause.Devices = new List<XDevice>();
					clause.Zones = new List<XZone>();
					clause.Directions = new List<XDirection>();

					var zoneUIDs = new List<Guid>();
					foreach (var zoneUID in clause.ZoneUIDs)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							zoneUIDs.Add(zoneUID);
							clause.Zones.Add(zone);
							zone.DevicesInLogic.Add(device);
						}
					}
					clause.ZoneUIDs = zoneUIDs;

					var deviceUIDs = new List<Guid>();
					foreach (var deviceUID in clause.DeviceUIDs)
					{
						var clauseDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
						if (clauseDevice != null && !clauseDevice.IsNotUsed)
						{
							deviceUIDs.Add(deviceUID);
							clause.Devices.Add(clauseDevice);
							clauseDevice.DevicesInLogic.Add(device);
						}
					}
					clause.DeviceUIDs = deviceUIDs;

					var directionUIDs = new List<Guid>();
					foreach (var directionUID in clause.DirectionUIDs)
					{
						var direction = DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
						if (direction != null)
						{
							directionUIDs.Add(directionUID);
							clause.Directions.Add(direction);
							direction.OutputDevices.Add(device);
						}
					}
					clause.DirectionUIDs = directionUIDs;

					if (clause.Zones.Count > 0 || clause.Devices.Count > 0 || clause.Directions.Count > 0)
						clauses.Add(clause);
				}
			device.DeviceLogic.Clauses = clauses;
		}

		static void InitializeDirections()
		{
			foreach (var direction in DeviceConfiguration.Directions)
			{
                var directionDevices = new List<XDirectionDevice>();
                foreach (var directionDevice in direction.DirectionDevices)
                {
                    if (directionDevice.DeviceUID != Guid.Empty)
                    {
                        var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == directionDevice.DeviceUID);
                        directionDevice.Device = device;
                        if (device != null)
                        {
                            directionDevices.Add(directionDevice);
                            direction.InputDevices.Add(device);
                            device.Directions.Add(direction);
                        }
                    }
                }
                direction.DirectionDevices = directionDevices;

                var directionZones = new List<XDirectionZone>();
                foreach (var directionZone in direction.DirectionZones)
                {
                    if (directionZone.ZoneUID != Guid.Empty)
                    {
                        var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == directionZone.ZoneUID);
                        directionZone.Zone = zone;
                        if (zone != null)
                        {
                            directionZones.Add(directionZone);
                            direction.InputZones.Add(zone);
                            zone.Directions.Add(direction);
                        }
                    }
                }
                direction.DirectionZones = directionZones;
			}
		}

		static void UpdateGKChildrenDescription()
		{
			foreach (var gkDevice in DeviceConfiguration.RootDevice.Children)
			{
				UpdateGKPredefinedName(gkDevice);
			}
		}

		public static void UpdateGKPredefinedName(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GK && device.Children.Count >= 15)
			{
				SetPredefinedName(device.Children[0], "");
				SetPredefinedName(device.Children[0], "Индикатор Неисправность");
				SetPredefinedName(device.Children[1], "Индикатор Пожар 1");
				SetPredefinedName(device.Children[2], "Индикатор Пожар 2");
				SetPredefinedName(device.Children[3], "Индикатор Внимание");
				SetPredefinedName(device.Children[4], "Индикатор Включение ПУСК");
				SetPredefinedName(device.Children[5], "Индикатор Тест");
				SetPredefinedName(device.Children[6], "Индикатор Отключение");
				SetPredefinedName(device.Children[7], "Индикатор Автоматика отключена");
				SetPredefinedName(device.Children[8], "Индикатор Звук отключен");
				SetPredefinedName(device.Children[9], "Индикатор Останов пуска");
				SetPredefinedName(device.Children[10], "Выход 1");
				SetPredefinedName(device.Children[11], "Выход 2");
				SetPredefinedName(device.Children[12], "Реле 1");
				SetPredefinedName(device.Children[13], "Реле 2");
				SetPredefinedName(device.Children[14], "Реле 3");
			}
		}
		static void SetPredefinedName(XDevice device, string name)
		{
			if (string.IsNullOrEmpty(device.PredefinedName))
				device.PredefinedName = name;
		}
	}
}