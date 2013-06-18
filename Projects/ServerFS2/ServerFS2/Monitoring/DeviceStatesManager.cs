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
		public static void Initialize()
		{
			//var systemDatabaseCreator = new SystemDatabaseCreator();
			//systemDatabaseCreator.Run();

			//foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			//{
			//    device.DeviceState = new DeviceState();
			//}
		}

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

		public static void GetStates()
		{
			foreach (var panelDevice in ConfigurationManager.Devices.Where(x => x.Driver.IsPanel))
			{
				if (panelDevice.Driver.DriverType == DriverType.IndicationBlock || panelDevice.Driver.DriverType == DriverType.PDU || panelDevice.Driver.DriverType == DriverType.PDU_PT)
					continue;

				Trace.WriteLine(panelDevice.PresentationAddressAndName);
				var remoteDeviceConfiguration = GetConfigurationOperationHelper.GetDeviceConfig(panelDevice);
				remoteDeviceConfiguration.Update();
				foreach (var remoteDevice in remoteDeviceConfiguration.Devices)
				{
					if (remoteDevice.ParentPanel == null)
						continue;

					var device = ConfigurationManager.Devices.FirstOrDefault(x => x.ParentPanel != null && x.ParentPanel == panelDevice && x.IntAddress == remoteDevice.IntAddress);
					device.StateWordOffset = remoteDevice.StateWordOffset;
					device.StateWordBytes = remoteDevice.StateWordBytes;
					device.RawParametersOffset = remoteDevice.RawParametersOffset;
					device.RawParametersBytes = remoteDevice.RawParametersBytes;
				}
				foreach (var device in ConfigurationManager.Devices)
				{
					if (device.StateWordBytes != null)
					{
						ParceDeviceState(device, device.StateWordBytes, device.RawParametersBytes);


						//foreach (var deviceDriverState in device.DeviceState.States)
						//{
						//    Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name + " " + device.PresentationAddressAndName);
						//}
					}
				}
			}
		}

		public static void GetStates(Device panelDevice, bool isSilent = false)
		{
			if (!IsMonitoringable(panelDevice))
				return;

			Trace.WriteLine(panelDevice.PresentationAddressAndName);
			var remoteDeviceConfiguration = GetConfigurationOperationHelper.GetDeviceConfig(panelDevice);
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
				ParceDeviceState(device, device.StateWordBytes, device.RawParametersBytes, isSilent);
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

		static void ParceDeviceState(Device device, List<byte> stateWordBytes, List<byte> rawParametersBytes, bool isSilent = false)
		{
			var stateWordBitArray = new BitArray(stateWordBytes.ToArray());
			var rawParametersBytesArray = new byte[] { rawParametersBytes[1], rawParametersBytes[0] };
			var rawParametersBitArray = new BitArray(rawParametersBytesArray);
			//if (device.AddressOnShleif == 200)
			{
				Trace.WriteLine("GetStates " + device.DottedPresentationNameAndAddress + " - " + BytesHelper.BytesToString(stateWordBytes) + " " + BytesHelper.BytesToString(rawParametersBytes));
				//int index = 0;
				//foreach (var bit in rawParametersBitArray)
				//{
				//    Trace.WriteLine(index.ToString() + " " + bit.ToString());
				//    index++;
				//}
			}

			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
					if (bitNo != -1 && bitNo < stateWordBitArray.Count)
					{
						var hasBit = stateWordBitArray[bitNo];
						SetStateByMetadata(device, metadataDeviceState, hasBit);
					}

					var intBitNo = MetadataHelper.GetIntBitNo(metadataDeviceState);
					if (intBitNo != -1 && intBitNo < rawParametersBitArray.Count)
					{
						var hasBit = rawParametersBitArray[intBitNo];
						SetStateByMetadata(device, metadataDeviceState, hasBit);
					}

				}
			}

			ChangeDeviceStates(device, device.DeviceState.States, isSilent);

			//foreach (var deviceDriverState in device.DeviceState.States)
			//{
			//    Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name);
			//}
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
					ParceDeviceState(device, StateWordBytes, RawParametersBytes);
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
					ParceDeviceState(device, StateWordBytes, RawParametersBytes);
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
			if (!isSilent)
			{
				NotifyStateChanged(device);
			}
		}

		public static void NotifyStateChanged(Device device)
		{
			//CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = new List<DeviceState>() { device.DeviceState } });
			device.DeviceState.OnStateChanged();
			//ZoneStateManager.ChangeOnDeviceState(device);
		}

		public static void AllToInitializing()
		{
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x =>
			{
				var state = x.Driver.States.FirstOrDefault(y => y.Name == "Устройство инициализируется");
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = state, Time = DateTime.Now } };
				NotifyStateChanged(x);
			});
		}

		public static void AllFromInitializing()
		{
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x =>
			{
				var state = x.Driver.States.FirstOrDefault(y => y.Name == "Устройство инициализируется");
				x.DeviceState.States.RemoveAll(y => y.DriverState == state);
				NotifyStateChanged(x);
			});
		}
	}
}