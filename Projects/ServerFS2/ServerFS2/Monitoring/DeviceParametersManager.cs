using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;
using System.Diagnostics;
using ServerFS2.Service;

namespace ServerFS2.Monitoring
{
	public static class DeviceParametersManager
	{
		static DateTime startTime;
		public static void UpdateCurrentState(Device device)
		{
			if (device == null)
			{
				return;
			}
			bool paramsChanged = false;
			List<byte> data = null;
			if (device.Driver.DriverType == DriverType.SmokeDetector || device.Driver.DriverType == DriverType.HandDetector)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
				if (data == null)
					return;
				device.StateWordBytes = data.GetRange(0, 2);
				if (device.Driver.DriverType == DriverType.SmokeDetector)
				{
					ChangeSmokiness(device, ref paramsChanged);
					ChangeDustiness(device, data[8], ref paramsChanged);
				}
				if (device.Driver.DriverType == DriverType.HeatDetector)
				{
					ChangeTemperature(device, data[8], ref paramsChanged);
				}
			}
			else if (device.Driver.DriverType == DriverType.CombinedDetector)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 11);
				if (data == null)
					return;
				device.StateWordBytes = data.GetRange(0, 2);
				{
					ChangeSmokiness(device, ref paramsChanged);
					ChangeDustiness(device, data[9], ref paramsChanged);
					ChangeTemperature(device, data[10], ref paramsChanged);
				}
			}
			else
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
				if (data == null)
					return;
			}
			var stateWordBytes = data.GetRange(0, 2);
			if (device.StateWordBytes != stateWordBytes)
			{
				device.StateWordBytes = stateWordBytes;
				DeviceStatesManager.ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
			}
			else if (paramsChanged)
			{
				CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = new List<DeviceState>() { device.DeviceState } });
				device.DeviceState.OnStateChanged();
			}

			if (device.IntAddress == 2 * 256 + 6)
			{
				Trace.WriteLine((DateTime.Now - startTime).TotalSeconds);
				startTime = DateTime.Now;
				Trace.WriteLine("Smokiness " + device.DeviceState.Smokiness);
				Trace.WriteLine("Dustiness " + device.DeviceState.Dustiness);
				Trace.WriteLine("Temperature " + device.DeviceState.Temperature);
			}
		}

		static void ChangeSmokiness(Device device, ref bool paramsChanged)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				paramsChanged = true;
			}
		}

		static void ChangeDustiness(Device device, byte dustinessByte, ref bool paramsChanged)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				paramsChanged = true;
			}
		}

		static void ChangeTemperature(Device device, byte temperatureByte, ref bool paramsChanged)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				paramsChanged = true;
			}
		}
	}
}