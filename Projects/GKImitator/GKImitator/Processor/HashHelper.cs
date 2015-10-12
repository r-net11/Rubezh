using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhAPI.GK;
using System.Security.Cryptography;

namespace GKImitator.Processor
{
	public static class HashHelper
	{
		public static List<byte> CreateHash1(GKDeviceConfiguration deviceConfiguration, GKDevice gkControllerDevice)
		{
			deviceConfiguration.UpdateConfiguration();
			deviceConfiguration.PrepareDescriptors();
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("devices:");
			foreach (var device in deviceConfiguration.Devices)
			{
				if (device.IsRealDevice && device.GKParent == gkControllerDevice)
					stringBuilder.Append(device.PresentationName).Append("@");
			}
			stringBuilder.Append("zones:");
			foreach (var zone in deviceConfiguration.Zones)
			{
				if (zone.GkDatabaseParent == gkControllerDevice)
					stringBuilder.Append(zone.PresentationName).Append("@");
			}
			stringBuilder.Append("directions:");
			foreach (var direction in deviceConfiguration.Directions)
			{
				if (direction.GkDatabaseParent == gkControllerDevice)
					stringBuilder.Append(direction.PresentationName).Append("@");
			}
			stringBuilder.Append("pumpStations:");
			foreach (var pumpStation in deviceConfiguration.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == gkControllerDevice)
				{
					stringBuilder.Append(pumpStation.PresentationName).Append("@");
					if (pumpStation.NSDevices != null)
					{
						stringBuilder.Append("nsDevices:");
						foreach (var nsDevice in pumpStation.NSDevices)
						{
							if (nsDevice.GKParent == gkControllerDevice)
								stringBuilder.Append(nsDevice.PresentationName).Append("@");
						}
					}
				}
			}
			stringBuilder.Append("mpts:");
			foreach (var mpt in deviceConfiguration.MPTs)
			{
				if (mpt.GkDatabaseParent == gkControllerDevice)
				{
					stringBuilder.Append(mpt.PresentationName).Append("@");
					if (mpt.MPTDevices != null)
					{
						stringBuilder.Append("nsDevices:");
						foreach (var mptDevice in mpt.MPTDevices)
						{
							if (mptDevice.Device.GKParent == gkControllerDevice)
								stringBuilder.Append(mptDevice.Device.PresentationName).Append("@");
						}
					}
				}
			}
			stringBuilder.Append("delays:");
			foreach (var delay in deviceConfiguration.Delays)
			{
				if (delay.GkDatabaseParent == gkControllerDevice)
				{
					stringBuilder.Append(delay.PresentationName).Append("@");
				}
			}
			stringBuilder.Append("guardZones:");
			foreach (var guardZone in deviceConfiguration.GuardZones)
			{
				if (guardZone.GkDatabaseParent == gkControllerDevice)
					stringBuilder.Append(guardZone.PresentationName).Append("@");
			}
			stringBuilder.Append("codes:");
			foreach (var code in deviceConfiguration.Codes)
			{
				if (code.GkDatabaseParent == gkControllerDevice)
					stringBuilder.Append(code.PresentationName).Append("@");
			}
			stringBuilder.Append("door:");
			foreach (var door in deviceConfiguration.Doors)
			{
				if (door.GkDatabaseParent == gkControllerDevice)
					stringBuilder.Append(door.PresentationName).Append("@");
			}
			return SHA256.Create().ComputeHash(Encoding.GetEncoding(1251).GetBytes(stringBuilder.ToString())).ToList();
		}
	}
}