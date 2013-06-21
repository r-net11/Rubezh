using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using Rubezh2010;
using ServerFS2.Service;

namespace ServerFS2.Monitoring
{
	public class DeviceStatesManager
	{
		public static void UpdatePanelState(Device panel, bool isSilent = false)
		{
			var states = new List<DeviceDriverState>();
			var statusBytes = ServerHelper.GetDeviceStatus(panel);
			if (statusBytes.Count < 8)
				return;
			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4]  };
			var bitArray = new BitArray(statusBytesArray);
			for (int i = 0; i < bitArray.Count; i++)
			{
				if (bitArray[i])
				{
					var metadataDeviceState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.no == i.ToString());
					var state = panel.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
					states.Add(new DeviceDriverState { DriverState = state, Time = DateTime.Now });
				}
			}
			ChangeDeviceStates(panel, states, isSilent);
			UpdateDeviceStateOnPanelState(panel, bitArray, isSilent);
		}

		public static void UpdatePDUPanelState(Device panel, bool isSilent = false)
		{
			var bytes = ServerHelper.GetDeviceStatus(panel);
			Trace.WriteLine(panel.PresentationAddressAndName + " " + BytesHelper.BytesToString(bytes));
		}

		public static void GetAllStates()
		{
			foreach (var device in ConfigurationManager.Devices)
			{
				if (device.ParentPanel != null && device.ParentPanel.IntAddress == 15)
				{
					var stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					if (stateBytes == null)
					{
						Trace.WriteLine("GetAllStates Failed " + device.DottedPresentationNameAndAddress);
						continue;
					}
					var deviceState = BytesHelper.SubstructShort(stateBytes, 0);
					Trace.WriteLine("GetAllStates " + device.DottedPresentationNameAndAddress + " - " + deviceState.ToString());
				}
			}
		}

		public static void GetStates(Device panelDevice, bool isSilent = false)
		{
			if (!IsMonitoringable(panelDevice))
				return;

			Trace.WriteLine(panelDevice.PresentationAddressAndName);
			var getConfigurationOperationHelper = new GetConfigurationOperationHelper(true);
			var remoteDeviceConfiguration = getConfigurationOperationHelper.GetDeviceConfig(panelDevice);
			remoteDeviceConfiguration.Update();
			foreach (var remoteDevice in remoteDeviceConfiguration.Devices)
			{
				if (remoteDevice.ParentPanel == null)
					continue;

				var device = ConfigurationManager.Devices.FirstOrDefault(x => x.ParentPanel != null && x.ParentPanel == panelDevice && x.IntAddress == remoteDevice.IntAddress);

				if (device == null)
					continue;

				device.StateWordOffset = remoteDevice.StateWordOffset;
				device.StateWordBytes = remoteDevice.StateWordBytes;
				device.RawParametersOffset = remoteDevice.RawParametersOffset;
				device.RawParametersBytes = remoteDevice.RawParametersBytes;
				ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes, isSilent);
			}
		}

		public static bool IsMonitoringable(Device device)
		{
			return device.Driver.IsPanel &&
				!device.IsMonitoringDisabled &&
				!(device.Driver.DriverType == DriverType.IndicationBlock ||
					device.Driver.DriverType == DriverType.PDU ||
					device.Driver.DriverType == DriverType.PDU_PT ||
					device.Driver.DriverType == DriverType.BUNS || 
					device.Driver.DriverType == DriverType.BUNS_2);
		}

		public static bool IsPDUMonitoringable(Device device)
		{
			return !device.IsMonitoringDisabled &&
				(device.Driver.DriverType == DriverType.IndicationBlock ||
					device.Driver.DriverType == DriverType.PDU ||
					device.Driver.DriverType == DriverType.PDU_PT ||
					device.Driver.DriverType == DriverType.UOO_TL ||
					device.Driver.DriverType == DriverType.MS_3 ||
					device.Driver.DriverType == DriverType.MS_4);
		}

		static void ParseDeviceState(Device device, List<byte> stateWordBytes, List<byte> rawParametersBytes, bool isSilent = false)
		{
			BitArray stateWordBitArray = null;
			BitArray rawParametersBitArray = null;
			if(stateWordBytes.Count > 0)
				stateWordBitArray = new BitArray(stateWordBytes.ToArray());
			if(rawParametersBytes.Count > 0)
				rawParametersBitArray = new BitArray(new byte[] { rawParametersBytes[1], rawParametersBytes[0] });
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
					if (stateWordBitArray != null && bitNo != -1 && bitNo < stateWordBitArray.Count)
					{
						var hasBit = stateWordBitArray[bitNo];
						SetStateByMetadata(device, metadataDeviceState, hasBit);
					}

					var intBitNo = MetadataHelper.GetIntBitNo(metadataDeviceState);
					if (rawParametersBitArray != null && intBitNo != -1 && intBitNo < rawParametersBitArray.Count)
					{
						var hasBit = rawParametersBitArray[intBitNo];
						SetStateByMetadata(device, metadataDeviceState, hasBit);
					}

				}
			}
			ChangeDeviceStates(device, device.DeviceState.States, isSilent);
		}

		static void SetStateByMetadata(Device device, driverConfigDeviceStatesDeviceState metadataDeviceState, bool hasBit)
		{
			if (hasBit)
			{
				if (!device.DeviceState.States.Any(x => x.DriverState.Code == metadataDeviceState.ID))
				{
					var driverState = device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
					if (driverState != null)
					{
						var deviceDriverState = new DeviceDriverState()
						{
							DriverState = driverState,
							Time = DateTime.Now
						};
						device.DeviceState.States.Add(deviceDriverState);
					}
				}
			}
			else
			{
				device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID);
			}
		}

		public static void UpdateDeviceState(List<FS2JournalItem> journalItems)
		{
			// check panel status

			foreach (var journalItem in journalItems)
			{
				if (journalItem != null && journalItem.Device != null)
				{
					var device = journalItem.Device;
					var StateWordBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					var RawParametersBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.RawParametersOffset, 2);
					ParseDeviceState(device, StateWordBytes, RawParametersBytes);
				}
			}
			//journalItem.Device.DeviceState.States = new List<DeviceDriverState>();
			//Trace.WriteLine(journalItem.Device.DottedPresentationNameAndAddress + " - " + journalItem.StateWord.ToString());
		}

		public static void UpdateAllDevicesOnPanelState(Device panel)
		{
			Trace.WriteLine("#################################################################################################################");
			foreach (var device in panel.Children)
			{
				try
				{
					var StateWordBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 2);
					var RawParametersBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.RawParametersOffset, 2);
					ParseDeviceState(device, StateWordBytes, RawParametersBytes);
					device.DeviceState.OnStateChanged();
				}
				catch
				{
					Trace.WriteLine("UpdateDeviceState failed" + device.PresentationAddressAndName);
				}

			}
		}

		public static void UpdateDeviceStateJournal(List<FS2JournalItem> journalItems)
		{
			// check panel status

			foreach (var journalItem in journalItems)
			{
				if (journalItem != null && journalItem.Device != null)
				{
					//var metadataDeviceTable = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => new Guid(x.deviceDriverID) == journalItem.Device.DriverUID);
					driverConfigDeviceTablesDeviceTable metadataDeviceTable = null;
					foreach (var metadataDeviceTableItem in MetadataHelper.Metadata.deviceTables)
					{
						if (metadataDeviceTableItem.deviceDriverID == null)
							continue;
						var guid = new Guid(metadataDeviceTableItem.deviceDriverID);
						var journalItemGuid = journalItem.Device.DriverUID;
						if (guid == journalItemGuid)
						{
							metadataDeviceTable = metadataDeviceTableItem;
							break;
						}
					}
					if (metadataDeviceTable != null)
					{
						foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
						{
							if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == metadataDeviceTable.tableType)
							{
								if (metadataDeviceState.enter != null)
								{
									foreach (var deviceStateEnter in metadataDeviceState.enter)
									{
										var eventValue = MetadataHelper.GetDeviceStateEventEnter(deviceStateEnter, journalItem.EventChoiceNo);
										if (eventValue != null)
										{
											if (eventValue == "$" + journalItem.EventCode.ToString("X2"))
											{
												var driverState = journalItem.Device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
												if (driverState != null)
												{
													if (!journalItem.Device.DeviceState.States.Any(x => x.DriverState != null && x.DriverState.Code == driverState.Code))
													{
														if (driverState.Name == "Включение РМ")
														{
															Trace.WriteLine("UpdateDeviceStateJournal == Включение РМ");
															;
														}

														var deviceDriverState = new DeviceDriverState()
														{
															DriverState = driverState,
															Time = DateTime.Now
														};
														journalItem.Device.DeviceState.States.Add(deviceDriverState);
													}
												}
											}
										}
									}

									foreach (var deviceStateLeave in metadataDeviceState.leave)
									{
										var eventValue = MetadataHelper.GetDeviceStateEventLeave(deviceStateLeave, journalItem.EventChoiceNo);
										if (eventValue != null)
										{
											if (eventValue == "$" + journalItem.EventCode.ToString("X2"))
											{
												var driverState = journalItem.Device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
												if (driverState != null)
												{
													var deviceDriverState = journalItem.Device.DeviceState.States.FirstOrDefault(x => x.DriverState.Code == driverState.Code);
													if (deviceDriverState != null)
													{
														journalItem.Device.DeviceState.States.Remove(deviceDriverState);
													}
												}
											}
										}
									}
								}
							}
						}
					}

					ChangeDeviceStates(journalItem.Device, journalItem.Device.DeviceState.States);
				}
				//journalItem.Device.DeviceState.States = new List<DeviceDriverState>();
				//Trace.WriteLine(journalItem.Device.DottedPresentationNameAndAddress + " - " + journalItem.StateWord.ToString());
			}
			// read device 80 byte
		}

		public static void UpdateDeviceStateOnPanelState(Device panelDevice, BitArray bitArray, bool isSilent = false)
		{
			foreach (var device in panelDevice.GetRealChildren())
			{
				foreach (var metadataDeviceState in MetadataHelper.GetMetadataDeviceStaes(device))
				{
					if (metadataDeviceState.leave != null)
					{
						foreach (var leaveDeviceState in metadataDeviceState.leave)
						{
							if (leaveDeviceState.panelState != null)
							{
								var pabelBitNo = Int32.Parse(leaveDeviceState.panelState);
								var hasBit = bitArray[pabelBitNo];
								if (!hasBit)
								{
									if (device.DeviceState.States.Any(x => x.DriverState.Code == metadataDeviceState.ID))
									{
										device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID);
										ChangeDeviceStates(device, device.DeviceState.States, isSilent);
									}
								}
							}
						}
					}
				}
			}
		}

		static void ChangeDeviceStates(Device device, List<DeviceDriverState> states, bool isSilent = false)
		{
			device.DeviceState.States = states;
			device.DeviceState.SerializableStates = device.DeviceState.States;
			ZoneStateManager.ChangeOnDeviceState(isSilent);
			if (!isSilent)
			{
				CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = new List<DeviceState>() { device.DeviceState } });
				device.DeviceState.OnStateChanged();
			}
		}

		static void SetStateByName(string stateName, Device device, ref List<DeviceState> changedDeviceStates)
		{
			var state = device.Driver.States.FirstOrDefault(y => y.Name == stateName);
			var deviceDriverState = new DeviceDriverState { DriverState = state, Time = DateTime.Now };
			device.DeviceState.States = new List<DeviceDriverState> { deviceDriverState };
			changedDeviceStates.Add(device.DeviceState);
			device.DeviceState.OnStateChanged();
		}

		public static void SetMonitoringDisabled()
		{
			var changedDeviceStates = new List<DeviceState>();
			ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel && x.IsMonitoringDisabled).ToList().ForEach(x =>
				{
					SetStateByName("Мониторинг устройства отключен", x, ref changedDeviceStates);
					ConfigurationManager.DeviceConfiguration.Devices.Where(y => y.ParentPanel == x).ToList().ForEach(y => SetStateByName("Мониторинг устройства отключен", y, ref changedDeviceStates));
				});
			CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = changedDeviceStates });
			ZoneStateManager.ChangeOnDeviceState();
		}

		public static void SetInitializingStateToAll()
		{
			var changedDeviceStates = new List<DeviceState>();
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x => SetStateByName("Устройство инициализируется", x, ref changedDeviceStates));
			CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = changedDeviceStates });
			ZoneStateManager.ChangeOnDeviceState();
		}

		public static void RemoveInitializingFromAll()
		{
			var changedDeviceStates = new List<DeviceState>();
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x =>
			{
				x.DeviceState.States.RemoveAll(y => y.DriverState.Name == "Устройство инициализируется");
				changedDeviceStates.Add(x.DeviceState);
				x.DeviceState.OnStateChanged();
			});
			CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = changedDeviceStates});
			ZoneStateManager.ChangeOnDeviceState();
		}

		static DateTime startTime;
		public static void GetDeviceCurrentState(Device device)
		{
			if (device == null)
			{
				return;
			}
			bool paramsChanged = false;
			List<byte> data = new List<byte>();
			if (device.Driver.DriverType == DriverType.SmokeDetector || device.Driver.DriverType == DriverType.HandDetector)
			{
				data = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.StateWordOffset, 9);
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
			}
			var stateWordBytes = data.GetRange(0, 2);
			if (device.StateWordBytes != stateWordBytes)
			{
				device.StateWordBytes = stateWordBytes;
				ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
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