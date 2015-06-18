﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static GKDeviceConfiguration DeviceConfiguration { get; set; }
		public static GKDriversConfiguration DriversConfiguration { get; set; }
		public static GKDeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }

		static GKManager()
		{
			DeviceConfiguration = new GKDeviceConfiguration();
			DriversConfiguration = new GKDriversConfiguration();
			AutoGeneratedDelays = new List<GKDelay>();
			AutoGeneratedPims = new List<GKPim>();
		}

		public static List<GKDevice> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}
		public static List<GKZone> Zones
		{
			get { return DeviceConfiguration.Zones; }
		}
		public static List<GKDirection> Directions
		{
			get { return DeviceConfiguration.Directions; }
		}
		public static List<GKPumpStation> PumpStations
		{
			get { return DeviceConfiguration.PumpStations; }
		}
		public static List<GKMPT> MPTs
		{
			get { return DeviceConfiguration.MPTs; }
		}
		public static List<GKDriver> Drivers
		{
			get { return DriversConfiguration.Drivers; }
		}
		public static List<GKParameterTemplate> ParameterTemplates
		{
			get { return GKManager.DeviceConfiguration.ParameterTemplates; }
		}
		public static List<GKDelay> Delays
		{
			get { return DeviceConfiguration.Delays; }
		}
		public static List<GKGuardZone> GuardZones
		{
			get { return DeviceConfiguration.GuardZones; }
		}
		public static List<GKDoor> Doors
		{
			get { return DeviceConfiguration.Doors; }
		}
		public static List<GKSKDZone> SKDZones
		{
			get { return DeviceConfiguration.SKDZones; }
		}

		public static List<GKDelay> AutoGeneratedDelays { get; set; }
		public static List<GKPim> AutoGeneratedPims { get; set; }

		public static void SetEmptyConfiguration()
		{
			DeviceConfiguration = new GKDeviceConfiguration();
			var driver = Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			DeviceConfiguration.RootDevice = new GKDevice()
			{
				DriverUID = driver.UID
			};
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			DeviceConfiguration.UpdateConfiguration();
		}

		public static void PrepareDescriptors()
		{
			DeviceConfiguration.PrepareDescriptors();
		}

		public static ushort GetKauLine(GKDevice device)
		{
			ushort lineNo = 0;
			if (device.Driver.IsKau)
			{
				var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
				if (modeProperty != null)
				{
					return modeProperty.Value;
				}
			}
			return lineNo;
		}

		public static string GetIpAddress(GKDevice device)
		{
			GKDevice gkControllerDevice = null;
			switch (device.DriverType)
			{
				case GKDriverType.GK:
					gkControllerDevice = device;
					break;

				case GKDriverType.RSR2_KAU:
					gkControllerDevice = device.Parent;
					break;

				default:
					{
						Logger.Error("GKManager.GetIpAddress Получить IP адрес можно только у ГК или в КАУ");
						throw new Exception("Получить IP адрес можно только у ГК или в КАУ");
					}
			}
			var ipAddress = gkControllerDevice.GetGKIpAddress();
			return ipAddress;
		}

		public static bool IsManyGK()
		{
			return DeviceConfiguration.Devices.Where(x => x.DriverType == GKDriverType.GK).Count() > 1;
		}

		public static GKAdvancedLogic LogicToCopy { get; private set; }
		public static void CopyLogic(GKLogic sourceLogic, bool hasOnClause = false, bool hasOnNowClause = false, bool hasOffClause = false, bool hasOffNowClause = false, bool hasStopClause = false)
		{
			LogicToCopy = new GKAdvancedLogic(hasOnClause, hasOnNowClause, hasOffClause, hasOffNowClause, hasStopClause);
			if (hasOnClause)
				LogicToCopy.OnClausesGroup = sourceLogic.OnClausesGroup.Clone();
			if (hasOnNowClause)
				LogicToCopy.OnNowClausesGroup = sourceLogic.OnNowClausesGroup.Clone();
			if (hasOffClause)
			{
				LogicToCopy.OffClausesGroup = sourceLogic.OffClausesGroup.Clone();
				LogicToCopy.UseOffCounterLogic = sourceLogic.UseOffCounterLogic;
			}
			if (hasOffNowClause)
				LogicToCopy.OffNowClausesGroup = sourceLogic.OffNowClausesGroup.Clone();
			if (hasStopClause)
				LogicToCopy.StopClausesGroup = sourceLogic.StopClausesGroup.Clone();
		}

		public static string CompareLogic(GKAdvancedLogic targetLogic)
		{
			var result ="";
			if (LogicToCopy.HasOnClause && !targetLogic.HasOnClause)
				result += "\nЦелевой объект не содержит \"Условие Включения\"";
			if (LogicToCopy.HasOnNowClause && !targetLogic.HasOnNowClause)
				result += "\nЦелевой объект не содержит \"Условие Включения Немедленно\"";
			if (LogicToCopy.HasOffClause && !targetLogic.HasOffClause)
				result += "\nЦелевой объект не содержит \"Условие Выключения\"";
			if (LogicToCopy.HasOffNowClause && !targetLogic.HasOffNowClause)
				result += "\nЦелевой объект не содержит \"Условие Выключения Немедленно\"";
			if (LogicToCopy.HasStopClause && !targetLogic.HasStopClause)
				result += "\nЦелевой объект не содержит \"Условие Останова\"";

			if (!LogicToCopy.HasOnClause && targetLogic.HasOnClause)
				result += "\nЦелевой объект дополнительно содержит \"Условие Включения\"";
			if (!LogicToCopy.HasOnNowClause && targetLogic.HasOnNowClause)
				result += "\nЦелевой объект дополнительно содержит \"Условие Включения Немедленно\"";
			if (!LogicToCopy.HasOffClause && targetLogic.HasOffClause)
				result += "\nЦелевой объект дополнительно содержит \"Условие Выключения\"";
			if (!LogicToCopy.HasOffNowClause && targetLogic.HasOffNowClause)
				result += "\nЦелевой объект дополнительно содержит \"Условие Выключения Немедленно\"";
			if (!LogicToCopy.HasStopClause && targetLogic.HasStopClause)
				result += "\nЦелевой объект дополнительно содержит \"Условие Останова\"";
			return result;
		}

		public static GKLogic PasteLogic(GKAdvancedLogic targetLogic)
		{
			var gkLogic = new GKLogic();
			if (targetLogic.HasOnClause)
				gkLogic.OnClausesGroup = LogicToCopy.OnClausesGroup;
			if (targetLogic.HasOnNowClause)
				gkLogic.OnNowClausesGroup = LogicToCopy.OnNowClausesGroup;
			if (targetLogic.HasOffClause)
			{
				gkLogic.OffClausesGroup = LogicToCopy.OffClausesGroup;
				targetLogic.UseOffCounterLogic = LogicToCopy.UseOffCounterLogic;
			}
			if (targetLogic.HasOffNowClause)
				gkLogic.OffNowClausesGroup = LogicToCopy.OffNowClausesGroup;
			if (targetLogic.HasStopClause)
				gkLogic.StopClausesGroup = LogicToCopy.StopClausesGroup;
			return gkLogic;
		}
	}
}