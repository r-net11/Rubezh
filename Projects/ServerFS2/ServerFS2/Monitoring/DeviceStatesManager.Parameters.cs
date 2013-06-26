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
		bool hasChanges = false;
		
		public void UpdateDeviceStateAndParameters(Device device)
		{
			hasChanges = false;
			List<byte> data;

			switch(device.Driver.DriverType)
			{
				case DriverType.RadioSmokeDetector:
				case DriverType.SmokeDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (data == null)
						return;
					ChangeSmokiness(device);
					ChangeDustiness(device, data[8]);
					break;
				case DriverType.HeatDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (data == null)
						return;
					ChangeTemperature(device, data[8]);
					break;
				case DriverType.CombinedDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 11);
					if (data == null)
						return;
					ChangeSmokiness(device);
					ChangeDustiness(device, data[9]);
					ChangeTemperature(device, data[10]);
					break;
				default:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					if (data == null)
						return;
					break;
			}
			ChangeStateWord(device, data);
			if (hasChanges)
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

		void ChangeSmokiness(Device device)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				ChangeParameter(device, smokiness, "Дым, дБ/м");
				hasChanges = true;
			}
		}

		void ChangeDustiness(Device device, byte dustinessByte)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				ChangeParameter(device, dustiness, "Пыль, дБ/м");
				hasChanges = true;
			}
		}

		void ChangeTemperature(Device device, byte temperatureByte)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				ChangeParameter(device, temperature, "Температура, °C");
				hasChanges = true;
			}
		}

		void ChangeStateWord(Device device, List<byte> data)
		{
			var stateWordBytes = data.GetRange(0, 2);
			if (device.StateWordBytes != stateWordBytes)
			{
				device.StateWordBytes = stateWordBytes;
				//hasChanges = true;
			}
		}

		public void ChangeParameter(Device device, float byteValue, string name)
		{
			var parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Caption == name);
			if (parameter == null)
			{
				var driverParameter = device.Driver.Parameters.FirstOrDefault(x => x.Caption == name);
				device.DeviceState.Parameters.Add(driverParameter);
				parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Caption == name);
			}
				
			if (parameter != null)
				parameter.Value = byteValue.ToString();
			else
				throw (new Exception("DeviceStatesManager.ChangeParameters"));
		}
	}  
}
