using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Service;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		public void UpdateDeviceStateAndParameters(Device device)
		{
			bool hasChanges = false;
			List<byte> data = null;
			if (device.Driver.DriverType == DriverType.SmokeDetector || device.Driver.DriverType == DriverType.HandDetector)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
				if (data == null)
					return;
				device.StateWordBytes = data.GetRange(0, 2);
				if (device.Driver.DriverType == DriverType.SmokeDetector)
				{
					ChangeSmokiness(device, ref hasChanges);
					ChangeDustiness(device, data[8], ref hasChanges);
				}
				if (device.Driver.DriverType == DriverType.HeatDetector)
				{
					ChangeTemperature(device, data[8], ref hasChanges);
				}
			}
			else if (device.Driver.DriverType == DriverType.CombinedDetector)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 11);
				if (data == null)
					return;
				device.StateWordBytes = data.GetRange(0, 2);
				{
					ChangeSmokiness(device, ref hasChanges);
					ChangeDustiness(device, data[9], ref hasChanges);
					ChangeTemperature(device, data[10], ref hasChanges);
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
				ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
			}
			else if (hasChanges)
			{
				device.DeviceState.SerializableStates = device.DeviceState.States;
				CallbackManager.DeviceParametersChanged(new List<DeviceState>() { device.DeviceState });
				device.DeviceState.OnStateChanged();
			}

			if (device.IntAddress == 2 * 256 + 6)
			{
				Trace.WriteLine("Smokiness " + device.DeviceState.Smokiness);
				Trace.WriteLine("Dustiness " + device.DeviceState.Dustiness);
				Trace.WriteLine("Temperature " + device.DeviceState.Temperature);
			}
		}

		void ChangeSmokiness(Device device, ref bool hasChanges)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				hasChanges = true;
			}
		}

		void ChangeDustiness(Device device, byte dustinessByte, ref bool hasChanges)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				hasChanges = true;
			}
		}

		void ChangeTemperature(Device device, byte temperatureByte, ref bool hasChanges)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				hasChanges = true;
			}
		}
	}
}
