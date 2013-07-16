using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Service;
using System.Diagnostics;
using Common;
using System.Collections;
using FiresecAPI;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		bool HasChanges = false;

		public void UpdateDeviceStateAndParameters(Device device)
		{
			List<byte> stateBytes;
			HasChanges = false;

			switch (device.Driver.DriverType)
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
			device.StateWordBytes = stateBytes;
			device.RawParametersBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.RawParametersOffset, device.RawParametersBytes.Count);
			ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);

			UpdateExtraDeviceState(device);
			
			if (HasChanges)
			{
				device.DeviceState.SerializableStates = device.DeviceState.States;
				CallbackManager.DeviceParametersChanged(new List<DeviceState>() { device.DeviceState });
				device.DeviceState.OnStateChanged();
			}
		}

		void UpdateExtraDeviceState(Device device)
		{
			if (device.Driver.DriverType == DriverType.MDU)
			{
				;
			}
			var deviceTable = MetadataHelper.GetMetadataDeviceTable(device);
			if (deviceTable != null && deviceTable.detalization != null)
			{
				foreach (var metadataDetalization in deviceTable.detalization)
				{
					var intStateType = Int32.Parse(metadataDetalization.@class);
					if (device.DeviceState.StateType == (StateType)intStateType)
					{
						var rawParameterIndex = -1;
						if (metadataDetalization.source == null)
							rawParameterIndex = 0;
						switch (metadataDetalization.source)
						{
							case "Data_MDU_0x81L":
								rawParameterIndex = 0;
								break;

							case "Data_MDU_0x81H":
								rawParameterIndex = 1;
								break;
						}
						if (rawParameterIndex != -1)
						{
							if (device.RawParametersBytes.Count > rawParameterIndex)
							{
								var rawParameterValue = device.RawParametersBytes[rawParameterIndex];
								var statusBytesArray = new byte[] { (byte)rawParameterValue };
								var bitArray = new BitArray(statusBytesArray);

								var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetalization.dictionary);
								if (metadataDictionary != null)
								{
									foreach (var matadataBit in metadataDictionary.bit)
									{
										var bitNo = Int32.Parse(matadataBit.no);
										if (bitArray[bitNo])
										{
											var parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Name == "FailureType");
											if (parameter == null)
											{
												parameter = new Parameter()
												{
													Name = "FailureType",
													Visible = true
												};
												device.DeviceState.Parameters.Add(parameter);
											}
											if (parameter.Value != matadataBit.value)
											{
												HasChanges = true;
												parameter.Value = matadataBit.value;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void UpdatePanelExtraDevices(Device panel)
		{
			HasChanges = false;

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

			if (HasChanges)
				NotifyStateChanged(panel);
		}

		public void UpdatePanelParameters(Device panel)
		{
			HasChanges = false;

			var faultyCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Неисправность")).Count();
			SetParameter(panel, faultyCount, "Heиcпpaвныx локальных уcтpoйcтв");
			//Trace.WriteLine("faultyCount " + faultyCount);

			var externalCount = 0;
			SetParameter(panel, externalCount, "Внешних устройств");
			//Trace.WriteLine("externalCount " + externalCount);

			var totalCount = panel.GetRealChildren().Count;
			SetParameter(panel, totalCount, "Всего устройств");
			//Trace.WriteLine("totalCount " + totalCount);

			var bypassCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Аппаратный обход устройства")).Count();
			SetParameter(panel, bypassCount, "Обойденных устройств");
			//Trace.WriteLine("bypassCount " + bypassCount);

			var lostCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Потеря связи")).Count();
			SetParameter(panel, lostCount, "Потерянных устройств");
			//Trace.WriteLine("lostCount " + lostCount);

			var dustfilledCount = panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Code == "HighDustiness")).Count();
			SetParameter(panel, dustfilledCount, "Запыленных устройств");
			//Trace.WriteLine("dustfilledCount " + dustfilledCount);

			if (HasChanges)
				NotifyStateChanged(panel);
		}

		void UpdateSmokiness(Device device)
		{
			var smokiness = USBManager.Send(device.Parent, 0x01, 0x56, device.ShleifNo, device.AddressOnShleif).Bytes[0];
			if (device.DeviceState.Smokiness != smokiness)
			{
				device.DeviceState.Smokiness = smokiness;
				HasChanges = true;
				SetParameter(device, smokiness, "Дым, дБ/м");
			}
		}

		void UpdateDustiness(Device device, byte dustinessByte)
		{
			var dustiness = (float)dustinessByte / 100;
			if (device.DeviceState.Dustiness != dustiness)
			{
				device.DeviceState.Dustiness = dustiness;
				HasChanges = true;
				SetParameter(device, dustiness, "Пыль, дБ/м");
			}
		}

		void UpdateTemperature(Device device, byte temperatureByte)
		{
			var temperature = temperatureByte;
			if (device.DeviceState.Temperature != temperature)
			{
				device.DeviceState.Temperature = temperature;
				HasChanges = true;
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
				throw (new Exception("DeviceStatesManager.ChangeParameter parameter == null"));
			}

			if (parameter.Value != byteValue.ToString())
			{
				parameter.Value = byteValue.ToString();
				HasChanges = true;
			}
		}
	}
}