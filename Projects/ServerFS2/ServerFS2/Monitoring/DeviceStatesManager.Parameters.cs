using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Service;
using System.Diagnostics;
using Common;
using System.Collections;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		bool hasChanges = false;
		
		public void UpdateDeviceStateAndParameters(Device device)
		{
			List<byte> stateBytes;
			List<byte> rawParametersBytes;
			hasChanges = false;

			switch(device.Driver.DriverType)
			{
				case DriverType.RadioSmokeDetector:
				case DriverType.SmokeDetector:
					stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (stateBytes == null || stateBytes.Count < 9)
						return;
					UpdateSmokiness(device);
					UpdateDustiness(device, stateBytes[8]);
					break;
				case DriverType.HeatDetector:
					stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (stateBytes == null || stateBytes.Count < 9)
						return;
					UpdateTemperature(device, stateBytes[8]);
					break;
				case DriverType.CombinedDetector:
					stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 11);
					if (stateBytes == null || stateBytes.Count < 11)
						return;
					UpdateSmokiness(device);
					UpdateDustiness(device, stateBytes[9]);
					UpdateTemperature(device, stateBytes[10]);
					break;
				default:
					stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					if (stateBytes == null || stateBytes.Count < 2)
						return;
					break;
			}
			device.RawParametersBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.RawParametersOffset, device.RawParametersBytes.Count);

			ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
			//UpdateStateWord(device, stateBytes);
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

		public void UpdatePanelExtraDevices(Device panel)
		{
			hasChanges = false;

			var responce = USBManager.Send(panel, 0x01, 0x13);
			if (responce.HasError)
			{
				Logger.Error(responce.Error);
				return;
			}
			var extraDevicesBytes = responce.Bytes;

			if (extraDevicesBytes.Count > 1)
			{
				var extraDevicesVal = BytesHelper.ExtractShort(extraDevicesBytes, 0);
				SetParameter(panel, extraDevicesVal, "Лишних устройств");
			}

			if(hasChanges)
				NotifyStateChanged(panel);
			//Trace.WriteLine(panel.PresentationAddressAndName + " " + BytesHelper.BytesToString(extraDevicesBytes));
		}

		public void UpdatePanelParameters(Device panel)
		{
			hasChanges = false;
			
			var faultyCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Неисправность")).Count();
			SetParameter(panel, faultyCount, "Heиcпpaвныx локальных уcтpoйcтв");
			Trace.WriteLine("faultyCount " + faultyCount);

			var externalCount = 0;
			SetParameter(panel, externalCount, "Внешних устройств");
			Trace.WriteLine("externalCount " + externalCount);

			var totalCount = panel.GetRealChildren().Count;
			SetParameter(panel, totalCount, "Всего устройств");
			Trace.WriteLine("totalCount " + totalCount);

			var bypassCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Аппаратный обход устройства")).Count();
			SetParameter(panel, bypassCount, "Обойденных устройств");
			Trace.WriteLine("bypassCount " + bypassCount);

			var lostCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Потеря связи")).Count();
			SetParameter(panel, lostCount, "Потерянных устройств");
			Trace.WriteLine("lostCount " + lostCount);

			var dustfilledCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Code == "HighDustiness")).Count();
			SetParameter(panel, dustfilledCount, "Запыленных устройств");
			Trace.WriteLine("dustfilledCount " + dustfilledCount);

			if(hasChanges)
				NotifyStateChanged(panel);
		}
		
		void UpdateSmokiness(Device device)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				hasChanges = true;
				SetParameter(device, smokiness, "Дым, дБ/м");
			}
		}

		void UpdateDustiness(Device device, byte dustinessByte)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				hasChanges = true;
				SetParameter(device, dustiness, "Пыль, дБ/м");
			}
		}

		void UpdateTemperature(Device device, byte temperatureByte)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				hasChanges = true;
				SetParameter(device, temperature, "Температура, °C");
			}
		}

		public void SetParameter(Device device, float byteValue, string name)
		{
			var parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Caption == name);
			if (parameter == null)
			{
				var driverParameter = device.Driver.Parameters.FirstOrDefault(x => x.Caption == name);
				device.DeviceState.Parameters.Add(driverParameter);
				parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Caption == name);
			}

			if (parameter == null)
			{
				throw(new Exception("DeviceStatesManager.ChangeParameter parameter == null"));
			}

			if (parameter.Value != byteValue.ToString())
			{
				parameter.Value = byteValue.ToString();
				hasChanges = true;
			}
		}

		void UpdateStateWord(Device device, List<byte> data)
		{
			//return;
			var states = new List<DeviceDriverState>();
			var stateWordBytesArray = data.GetRange(0, 2).ToArray();

			var bitArray = new BitArray(stateWordBytesArray);
			for (int i = 0; i < bitArray.Count; i++)
			{
				if (bitArray[i])
				{
					var metadataDeviceState = MetadataHelper.Metadata.deviceStates.FirstOrDefault(x => x.bitno == i.ToString() || x.bitNo == i.ToString() || x.Bitno == i.ToString());
					if (metadataDeviceState != null)
					{
						var state = device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
						if (state != null)
							states.Add(new DeviceDriverState { DriverState = state, Time = DateTime.Now });
					}
				}
			}
			if (SetNewDeviceStates(device, states))
			{
				ForseUpdateDeviceStates(device);
			}
		}
	}  
}
