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
			List<byte> data;
			hasChanges = false;
			switch(device.Driver.DriverType)
			{
				case DriverType.RadioSmokeDetector:
				case DriverType.SmokeDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (data == null || data.Count < 9)
						return;
					GetStateWord(device, data);
					GetSmokiness(device);
					GetDustiness(device, data[8]);
					break;
				case DriverType.HeatDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
					if (data == null || data.Count < 9)
						return;
					GetStateWord(device, data);
					GetTemperature(device, data[8]);
					break;
				case DriverType.CombinedDetector:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 11);
					if (data == null || data.Count < 11)
						return;
					GetStateWord(device, data);
					GetSmokiness(device);
					GetDustiness(device, data[9]);
					GetTemperature(device, data[10]);
					break;
				default:
					data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					if (data == null || data.Count < 2)
						return;
					GetStateWord(device, data);
					break;
			}
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
			
			var extraDevicesVal = extraDevicesBytes[0] * 256 + extraDevicesBytes[1];
			SetParameter(panel, extraDevicesVal, "Лишних устройств");

			if(hasChanges)
				NotifyStateChanged(panel);
			Trace.WriteLine(panel.PresentationAddressAndName + " " + BytesHelper.BytesToString(extraDevicesBytes));
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
		
		void GetSmokiness(Device device)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				hasChanges = true;
				SetParameter(device, smokiness, "Дым, дБ/м");
			}
		}

		void GetDustiness(Device device, byte dustinessByte)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				hasChanges = true;
				SetParameter(device, dustiness, "Пыль, дБ/м");
			}
		}

		void GetTemperature(Device device, byte temperatureByte)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				hasChanges = true;
				SetParameter(device, temperature, "Температура, °C");
			}
		}

		void GetStateWord(Device device, List<byte> data)
		{
			var states = new List<DeviceDriverState>();
			var stateWordBytesArray = data.GetRange(0, 2).ToArray();

			var bitArray = new BitArray(stateWordBytesArray);
			for (int i = 0; i < bitArray.Count; i++)
			{
				if (bitArray[i])
				{
					var metadataDeviceState = MetadataHelper.Metadata.deviceStates.FirstOrDefault(x => x.bitno == i.ToString() || x.bitNo == i.ToString() || x.Bitno == i.ToString());
					var state = device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
					if(state != null)
						states.Add(new DeviceDriverState { DriverState = state, Time = DateTime.Now });
				}
			}
			if (SetNewDeviceStates(device, states))
			{
				ChangeDeviceStates(device, true);
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
	}  
}
