using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common;

namespace FiresecClient
{
	public static partial class UpdateConfigurationHelper
	{
		public static XDeviceConfiguration DeviceConfiguration { get; private set; }

		public static void Update(XDeviceConfiguration deviceConfiguration)
		{
			DeviceConfiguration = deviceConfiguration;
			if (DeviceConfiguration == null)
			{
				DeviceConfiguration = new XDeviceConfiguration();
			}
			if (DeviceConfiguration.RootDevice == null)
			{
				var systemDriver = XManager.Drivers.FirstOrDefault(x => x.DriverType == XDriverType.System);
				if (systemDriver != null)
				{
					DeviceConfiguration.RootDevice = new XDevice()
					{
						DriverUID = systemDriver.UID,
						Driver = systemDriver
					};
				}
				else
				{
					Logger.Error("XManager.SetEmptyConfiguration systemDriver = null");
				}
			}
			DeviceConfiguration.ValidateVersion();

			DeviceConfiguration.Update();
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Driver = XManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}
			DeviceConfiguration.Reorder();

			InitializeProperties();
			Invalidate();
		}

		static void InitializeProperties()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				foreach (var property in device.Properties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				foreach (var property in device.DeviceProperties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				device.InitializeDefaultProperties();
			}
		}

		static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InitializeLogic();
			InitializeDirections();
			InitializePumpStations();
			InitializeGuardUsers();
			UpdateGKChildrenDescription();
		}

		static void ClearAllReferences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Zones = new List<XZone>();
				device.Directions = new List<XDirection>();
				device.NSDirections = new List<XDirection>();
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
				direction.NSDevices = new List<XDevice>();
			}
			foreach (var pumpStation in DeviceConfiguration.PumpStations)
			{
				pumpStation.InputZones = new List<XZone>();
				pumpStation.InputDevices = new List<XDevice>();
				pumpStation.InputDirections = new List<XDirection>();
				pumpStation.NSDevices = new List<XDevice>();
			}
		}

		static void InitializeDevicesInZone()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				var zoneUIDs = new List<Guid>();
				if (device.Driver.HasZone)
				{
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
				}
				device.ZoneUIDs = zoneUIDs;
			}
		}

		static void InitializeLogic()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				InvalidateOneLogic(DeviceConfiguration, device, device.DeviceLogic);
				if (device.NSLogic != null)
					InvalidateOneLogic(DeviceConfiguration, device, device.NSLogic);
			}
		}

		public static void InvalidateOneLogic(XDeviceConfiguration deviceConfiguration, XDevice device, XDeviceLogic deviceLogic)
		{
			var clauses = new List<XClause>();
			foreach (var clause in deviceLogic.Clauses)
			{
				clause.Devices = new List<XDevice>();
				clause.Zones = new List<XZone>();
				clause.Directions = new List<XDirection>();

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in clause.ZoneUIDs)
				{
					var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
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
					var clauseDevice = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
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
					var direction = deviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
					if (direction != null)
					{
						directionUIDs.Add(directionUID);
						clause.Directions.Add(direction);
						direction.OutputDevices.Add(device);
						device.Directions.Add(direction);
					}
				}
				clause.DirectionUIDs = directionUIDs;

				if (clause.Zones.Count > 0 || clause.Devices.Count > 0 || clause.Directions.Count > 0)
					clauses.Add(clause);
			}
			deviceLogic.Clauses = clauses;
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

				var nsDeviceUIDs = new List<Guid>();
				foreach (var nsDeviceUID in direction.NSDeviceUIDs)
				{
					var nsDevice = XManager.Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
					if (nsDevice != null)
					{
						switch (nsDevice.DriverType)
						{
							case XDriverType.AM1_T:
							case XDriverType.RSR2_Bush:
								nsDeviceUIDs.Add(nsDevice.UID);
								direction.NSDevices.Add(nsDevice);
								nsDevice.NSDirections.Add(direction);
								break;

							case XDriverType.Pump:
								if (nsDevice.IntAddress <= 8 || nsDevice.IntAddress == 12)
								{
									nsDeviceUIDs.Add(nsDevice.UID);
									direction.NSDevices.Add(nsDevice);
									nsDevice.NSDirections.Add(direction);
								}
								break;
						}
					}
				}
				direction.NSDeviceUIDs = nsDeviceUIDs;
			}
		}

		static void InitializePumpStations()
		{
			foreach (var pumpStation in DeviceConfiguration.PumpStations)
			{
				var nsDeviceUIDs = new List<Guid>();
				foreach (var nsDeviceUID in pumpStation.NSDeviceUIDs)
				{
					var nsDevice = XManager.Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
					if (nsDevice != null)
					{
						switch (nsDevice.DriverType)
						{
							case XDriverType.Pump:
								if (nsDevice.IntAddress <= 8 || nsDevice.IntAddress == 12)
								{
									nsDeviceUIDs.Add(nsDevice.UID);
									pumpStation.NSDevices.Add(nsDevice);
									//nsDevice.NSDirections.Add(pumpStation);
								}
								break;
						}
					}
				}
				pumpStation.NSDeviceUIDs = nsDeviceUIDs;
				InvalidatePumpStationLogic(pumpStation, pumpStation.StartLogic);
				InvalidatePumpStationLogic(pumpStation, pumpStation.ForbidLogic);
				if (pumpStation.AutomaticOffLogic == null)
					pumpStation.AutomaticOffLogic = new XDeviceLogic();
				InvalidatePumpStationLogic(pumpStation, pumpStation.AutomaticOffLogic);
			}
		}

		public static void InvalidatePumpStationLogic(XPumpStation pumpStation, XDeviceLogic deviceLogic)
		{
			var clauses = new List<XClause>();
			foreach (var clause in deviceLogic.Clauses)
			{
				clause.Devices = new List<XDevice>();
				clause.Zones = new List<XZone>();
				clause.Directions = new List<XDirection>();

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
						if (!pumpStation.InputDevices.Contains(clauseDevice))
							pumpStation.InputDevices.Add(clauseDevice);
						//clauseDevice.DevicesInLogic.Add(device);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in clause.ZoneUIDs)
				{
					var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zoneUID);
						clause.Zones.Add(zone);
						if (!pumpStation.InputZones.Contains(zone))
							pumpStation.InputZones.Add(zone);
						//zone.DevicesInLogic.Add(device);
					}
				}
				clause.ZoneUIDs = zoneUIDs;

				var directionUIDs = new List<Guid>();
				foreach (var directionUID in clause.DirectionUIDs)
				{
					var direction = DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
					if (direction != null)
					{
						directionUIDs.Add(directionUID);
						clause.Directions.Add(direction);
						if (!pumpStation.InputDirections.Contains(direction))
							pumpStation.InputDirections.Add(direction);
						//direction.OutputDevices.Add(device);
						//device.Directions.Add(direction);
					}
				}
				clause.DirectionUIDs = directionUIDs;

				if (clause.Zones.Count > 0 || clause.Devices.Count > 0 || clause.Directions.Count > 0)
					clauses.Add(clause);
			}
			deviceLogic.Clauses = clauses;
		}

		static void InitializeGuardUsers()
		{
			foreach (var guardUser in DeviceConfiguration.GuardUsers)
			{
				var zoneUIDs = new List<Guid>();
				guardUser.Zones = new List<XZone>();
				if (guardUser.ZoneUIDs != null)
					foreach (var zoneUID in guardUser.ZoneUIDs)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							guardUser.Zones.Add(zone);
							zoneUIDs.Add(zoneUID);
						}
					}
				guardUser.ZoneUIDs = zoneUIDs;
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
			if (device.DriverType == XDriverType.GK && device.Children.Count >= 15)
			{
				device.Children[0].PredefinedName = "Неисправность";
				device.Children[1].PredefinedName = "Пожар 1";
				device.Children[2].PredefinedName = "Пожар 2";
				device.Children[3].PredefinedName = "Внимание";
				device.Children[4].PredefinedName = "Включение ПУСК";
				device.Children[5].PredefinedName = "Тест";
				device.Children[6].PredefinedName = "Отключение";
				device.Children[7].PredefinedName = "Автоматика отключена";
				device.Children[8].PredefinedName = "Звук отключен";
				device.Children[9].PredefinedName = "Останов пуска";
				device.Children[10].PredefinedName = "Реле 1";
				device.Children[11].PredefinedName = "Реле 2";
				device.Children[12].PredefinedName = "Реле 3";
				device.Children[13].PredefinedName = "Реле 4";
				device.Children[14].PredefinedName = "Реле 5";
			}
		}
	}
}