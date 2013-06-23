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
			Trace.WriteLine(panelDevice.PresentationAddressAndName);
			var getConfigurationOperationHelper = new GetConfigurationOperationHelper(true);
			var remoteDeviceConfiguration = getConfigurationOperationHelper.GetDeviceConfig(panelDevice);
			remoteDeviceConfiguration.Update();
			foreach (var remoteDevice in remoteDeviceConfiguration.Devices)
			{
				if (remoteDevice.Driver.IsPanel || remoteDevice.ParentPanel == null)
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

		public static void ParseDeviceState(Device device, List<byte> stateWordBytes, List<byte> rawParametersBytes, bool isSilent = false)
		{
			if (stateWordBytes == null || rawParametersBytes == null)
				return;
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
						SetStateByMetadata(device, metadataDeviceState, hasBit, isSilent);
							
					}

					var intBitNo = MetadataHelper.GetIntBitNo(metadataDeviceState);
					if (rawParametersBitArray != null && intBitNo != -1 && intBitNo < rawParametersBitArray.Count)
					{
						var hasBit = rawParametersBitArray[intBitNo];
						SetStateByMetadata(device, metadataDeviceState, hasBit, isSilent);
					}
				}
			}
		}

		static void SetStateByMetadata(Device device, driverConfigDeviceStatesDeviceState metadataDeviceState, bool hasBit, bool isSilent)
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
						ChangeDeviceStates(device, null, isSilent);
					}
				}
			}
			else
			{
				if (device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID) > 0)
					ChangeDeviceStates(device, null, isSilent);
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
														var deviceDriverState = new DeviceDriverState()
														{
															DriverState = driverState,
															Time = DateTime.Now
														};
														journalItem.Device.DeviceState.States.Add(deviceDriverState);
														ChangeDeviceStates(journalItem.Device, null, false);
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
														ChangeDeviceStates(journalItem.Device, null, false);
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
									if (device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID) > 0)
									{
										ChangeDeviceStates(device, null, isSilent);
									}
								}
							}
						}
					}
				}
			}
		}

		static void ChangeDeviceStates(Device device, List<DeviceDriverState> newStates, bool isSilent)
		{
			if (newStates != null)
			{
				foreach (var state in newStates)
				{
					if (!device.DeviceState.States.Any(x => x.DriverState.Code == state.DriverState.Code))
					{
						var driverState = device.Driver.States.FirstOrDefault(x => x.Code == state.DriverState.Code);
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

				var satesToRemove = new List<DeviceDriverState>();
				foreach (var state in device.DeviceState.States)
				{
					if (!newStates.Any(x => x.DriverState.Id == state.DriverState.Id))
					{
						satesToRemove.Add(state);
					}
				}
				device.DeviceState.States.RemoveAll(x => satesToRemove.Any(y => x.DriverState.Id == x.DriverState.Id));
			}

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
			ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.IsParentMonitoringDisabled).ToList().ForEach(x =>
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
	}
}