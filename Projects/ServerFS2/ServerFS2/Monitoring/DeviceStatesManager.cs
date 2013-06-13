using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2;
using Rubezh2010;
using ServerFS2.ConfigurationWriter;
using System.Collections;
using Infrastructure.Common.Windows;
using ServerFS2.Service;
using ServerFS2.Processor;

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

		public static void UpdatePanelState(Device panel)
		{
			var states = new List<DeviceDriverState>();
			var statusBytes = ServerHelper.GetDeviceStatus(panel);
			if (statusBytes.Count < 8)
				return;
			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
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
			ChangeDeviceStates(panel, states);
			UpdateDeviceStateOnPanelState(panel, bitArray);
		}


		public static void GetAllStates()
		{
			foreach (var device in ConfigurationManager.Devices)
			{
				if (device.ParentPanel != null && device.ParentPanel.IntAddress == 15)
				{
					var stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.Offset, 2);
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
				var remoteDeviceConfiguration = ServerHelper.GetDeviceConfig(panelDevice);
				remoteDeviceConfiguration.Update();
				foreach (var remoteDevice in remoteDeviceConfiguration.Devices)
				{
					if (remoteDevice.ParentPanel == null)
						continue;

					var device = ConfigurationManager.Devices.FirstOrDefault(x => x.ParentPanel != null && x.ParentPanel == panelDevice && x.IntAddress == remoteDevice.IntAddress);
					device.Offset = remoteDevice.Offset;
					device.InnerDeviceParameters = remoteDevice.InnerDeviceParameters;
				}
				foreach (var device in ConfigurationManager.Devices)
				{
					if (device.InnerDeviceParameters != null)
					{
						ParceDeviceState(device, device.InnerDeviceParameters);


						//foreach (var deviceDriverState in device.DeviceState.States)
						//{
						//    Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name + " " + device.PresentationAddressAndName);
						//}
					}
				}
			}
		}

		public static void GetStates(Device panelDevice)
		{
			if (!IsMonitoringable(panelDevice))
				return;

			Trace.WriteLine(panelDevice.PresentationAddressAndName);
			var remoteDeviceConfiguration = ServerHelper.GetDeviceConfig(panelDevice);
			remoteDeviceConfiguration.Update();
			foreach (var remoteDevice in remoteDeviceConfiguration.Devices)
			{
				if (remoteDevice.ParentPanel == null)
					continue;

				var device = ConfigurationManager.Devices.FirstOrDefault(x => x.ParentPanel != null && x.ParentPanel == panelDevice && x.IntAddress == remoteDevice.IntAddress);

				if (device == null)
					continue;

				device.Offset = remoteDevice.Offset;
				device.InnerDeviceParameters = remoteDevice.InnerDeviceParameters;
				ParceDeviceState(device, device.InnerDeviceParameters);
			}
		}

		public static bool IsMonitoringable(Device device)
		{
			return device.Driver.IsPanel &&
				!(device.Driver.DriverType == DriverType.IndicationBlock ||
					device.Driver.DriverType == DriverType.PDU ||
					device.Driver.DriverType == DriverType.PDU_PT ||
					device.Driver.DriverType == DriverType.BUNS || 
					device.Driver.DriverType == DriverType.BUNS_2);
		}

		static void ParceDeviceState(Device device, List<byte> stateBytes)
		{
			var bitArray = new BitArray(stateBytes.ToArray());

			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
					if (bitNo != -1 && bitNo < bitArray.Count)
					{
						var hasBit = bitArray[bitNo];
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
				}
			}

			//var states = new List<DeviceDriverState>();
			//var tableNo = MetadataHelper.GetDeviceTableNo(device);
			//if (tableNo != null)
			//{
			//    var stateWord = BytesHelper.ExtractShort(stateBytes, 0);
			//    Trace.WriteLine("stateWord=" + stateWord);
			//    var bitArray = new BitArray(stateBytes.ToArray());


			//    foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			//    {
			//        if (metadataDeviceState.tableType != null && metadataDeviceState.tableType != tableNo)
			//            continue;
			//        if (metadataDeviceState.notForTableType != null && metadataDeviceState.notForTableType == tableNo)
			//            continue;

			//        var found = false;
			//        try
			//        {
			//            if (metadataDeviceState.bits != null)
			//            {
			//                var bitStrings = metadataDeviceState.bits.Split('-');
			//                var minBit = Int32.Parse(bitStrings[0]);
			//                var maxBit = Int32.Parse(bitStrings[2]);
			//                var bitsValue = (bitArray[minBit] ? 1 : 0) + (bitArray[maxBit] ? 1 : 0) * 2;
			//                if (bitsValue.ToString() == metadataDeviceState.value)
			//                    found = true;
			//            }
			//        }
			//        catch { }

			//        var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
			//        if (bitNo != -1)
			//        {
			//            found = bitArray[bitNo];
			//            if (metadataDeviceState.inverse != null && metadataDeviceState.inverse == "1")
			//                found = !found;
			//        }

			//        if (found)
			//        {
			//            var driverState = device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
			//            if (driverState != null)
			//            {
			//                var deviceDriverState = new DeviceDriverState()
			//                {
			//                    DriverState = driverState,
			//                    Time = DateTime.Now
			//                };
			//                states.Add(deviceDriverState);
			//                device.DeviceState.States.Add(deviceDriverState);
			//            }
			//        }
			//    }
			//}

			ChangeDeviceStates(device, device.DeviceState.States);

			foreach (var deviceDriverState in device.DeviceState.States)
			{
				Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name);
			}
			Trace.WriteLine("GetStates " + device.DottedPresentationNameAndAddress + " - " + BytesHelper.BytesToString(stateBytes));
		}

		public static void UpdateDeviceState(List<FS2JournalItem> journalItems)
		{
			// check panel status

			foreach (var journalItem in journalItems)
			{
				if (journalItem != null && journalItem.Device != null)
				{
					var device = journalItem.Device;
					var stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.Offset, 2);
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
					var stateBytes = ServerHelper.GetBytesFromFlashDB(panel, device.Offset, 2);
					ParceDeviceState(device, stateBytes);
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

		public static void UpdateDeviceStateOnPanelState(Device panelDevice, BitArray bitArray)
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
										ChangeDeviceStates(device, device.DeviceState.States);
									}
								}
							}
						}
					}
				}
			}
		}

		static void ChangeDeviceStates(Device device, List<DeviceDriverState> states)
		{
			device.DeviceState.States = states;
			device.DeviceState.SerializableStates = device.DeviceState.States;
			CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = new List<DeviceState>() { device.DeviceState } });
			device.DeviceState.OnStateChanged();

			ZoneStateManager.ChangeOnDeviceState(device);
		}

		public static void AllToInitializing()
		{
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x =>
			{
				var state = x.Driver.States.FirstOrDefault(y => y.Name == "Устройство инициализируется");
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = state, Time = DateTime.Now } };
				x.DeviceState.OnStateChanged();
			});
		}

		public static void AllFromInitializing()
		{
			ConfigurationManager.DeviceConfiguration.Devices.ForEach(x =>
			{
				var state = x.Driver.States.FirstOrDefault(y => y.Name == "Устройство инициализируется");
				x.DeviceState.States.RemoveAll(y => y.DriverState == state);
				x.DeviceState.OnStateChanged();
			});
		}
	}
}