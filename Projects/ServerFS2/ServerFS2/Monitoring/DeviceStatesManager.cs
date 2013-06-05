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

namespace ServerFS2.Monitor
{
	public class DeviceStatesManager
	{
		public static void Initialize()
		{
			//var systemDatabaseCreator = new SystemDatabaseCreator();
			//systemDatabaseCreator.Run();

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				device.DeviceState = new DeviceState();
			}
		}

		public static void GetAllStates()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
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
			foreach (var panelDevice in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel))
			{
				//if (panelDevice.IntAddress != 15)
				//    continue;

				if (panelDevice.Driver.DriverType == DriverType.IndicationBlock || panelDevice.Driver.DriverType == DriverType.PDU || panelDevice.Driver.DriverType == DriverType.PDU_PT)
					continue;

				var deviceCongiguration = ServerHelper.GetDeviceConfig(panelDevice);
				foreach (var device in deviceCongiguration.Devices)
				{
					device.DeviceState = new DeviceState();
				}
				foreach (var device in deviceCongiguration.Devices)
				{
					if (device.InnerDeviceParameters != null)
					{
						ParceDeviceState(device, device.InnerDeviceParameters);

						var stringParameters = "";
						foreach (var b in device.InnerDeviceParameters)
						{
							stringParameters += b.ToString() + " ";
						}
						Trace.WriteLine("GetStates " + device.DottedPresentationNameAndAddress + " - " + stringParameters);
						foreach (var deviceDriverState in device.DeviceState.States)
						{
							Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name);
						}
					}
				}
			}
		}

		static void ParceDeviceState(Device device, List<byte> stateBytes)
		{
			device.DeviceState.States = new List<DeviceDriverState>();

			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			if (tableNo != null)
			{
				var stateWord = BytesHelper.ExtractShort(stateBytes, 0);
				Trace.WriteLine("stateWord=" + stateWord);
				var bitArray = new BitArray(stateBytes.ToArray());
				for (int i = 0; i < 8; i++)
				{
					foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
					{
						if ((metadataDeviceState.tableType != null && metadataDeviceState.tableType == tableNo) ||
							(metadataDeviceState.tableType == null))
						//(metadataDeviceState.tableType == null && metadataDeviceState.notForTableType != null && metadataDeviceState.notForTableType != tableNo))
						{
							if (metadataDeviceState.bitno == i.ToString() ||
								metadataDeviceState.bitNo == i.ToString() ||
								metadataDeviceState.Bitno == i.ToString())
							//metadataDeviceState.intBitno == i.ToString() ||
							//metadataDeviceState.Intbitno == i.ToString())
							{
								var isBitSetted = bitArray[i];
								if (metadataDeviceState.inverse != null && metadataDeviceState.inverse == "1")
									isBitSetted = !isBitSetted;

								if (isBitSetted)
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
						}
					}
				}
			}

			Trace.WriteLine("Monitoring GetStates " + device.DottedPresentationNameAndAddress);
			foreach (var deviceDriverState in device.DeviceState.States)
			{
				Trace.WriteLine("deviceDriverState " + deviceDriverState.DriverState.Name);
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
					var stateBytes = ServerHelper.GetBytesFromFlashDB(device.ParentPanel, device.Offset, 2);
					ParceDeviceState(device, stateBytes);
				}
			}
			//journalItem.Device.DeviceState.States = new List<DeviceDriverState>();
			//Trace.WriteLine(journalItem.Device.DottedPresentationNameAndAddress + " - " + journalItem.StateWord.ToString());
		}

		//public static void UpdateDeviceState(List<FS2JournalItem> journalItems)
		//{
		//    // check panel status

		//    foreach (var journalItem in journalItems)
		//    {
		//        if (journalItem != null && journalItem.Device != null)
		//        {
		//            //var metadataDeviceTable = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => new Guid(x.deviceDriverID) == journalItem.Device.DriverUID);
		//            driverConfigDeviceTablesDeviceTable metadataDeviceTable = null;
		//            foreach (var metadataDeviceTableItem in MetadataHelper.Metadata.deviceTables)
		//            {
		//                if (metadataDeviceTableItem.deviceDriverID == null)
		//                    continue;
		//                var guid = new Guid(metadataDeviceTableItem.deviceDriverID);
		//                var journalItemGuid = journalItem.Device.DriverUID;
		//                if (guid == journalItemGuid)
		//                {
		//                    metadataDeviceTable = metadataDeviceTableItem;
		//                    break;
		//                }
		//            }
		//            if (metadataDeviceTable != null)
		//            {
		//                foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
		//                {
		//                    if (metadataDeviceState.tableType == metadataDeviceTable.tableType)
		//                    {
		//                        if (metadataDeviceState.enter != null)
		//                        {
		//                            foreach (var deviceStateEnter in metadataDeviceState.enter)
		//                            {
		//                                var eventValue = MetadataHelper.GetDeviceStateEventEnter(deviceStateEnter, journalItem.EventChoiceNo);
		//                                if (eventValue != null)
		//                                {
		//                                    if (eventValue == "$" + journalItem.EventCode.ToString("X2"))
		//                                    {
		//                                        var driverState = journalItem.Device.Driver.States.FirstOrDefault(x => x.Id == metadataDeviceState.ID);
		//                                        if (driverState != null)
		//                                        {
		//                                            if (!journalItem.Device.DeviceState.States.Any(x => x.DriverState.Id == driverState.Id))
		//                                            {
		//                                                var deviceDriverState = new DeviceDriverState()
		//                                                {
		//                                                    DriverState = driverState,
		//                                                    Time = DateTime.Now
		//                                                };
		//                                                journalItem.Device.DeviceState.States.Add(deviceDriverState);
		//                                            }
		//                                        }
		//                                    }
		//                                }
		//                            }

		//                            foreach (var deviceStateLeave in metadataDeviceState.leave)
		//                            {
		//                                var eventValue = MetadataHelper.GetDeviceStateEventLeave(deviceStateLeave, journalItem.EventChoiceNo);
		//                                if (eventValue != null)
		//                                {
		//                                    if (eventValue == "$" + journalItem.EventCode.ToString("X2"))
		//                                    {
		//                                        var driverState = journalItem.Device.Driver.States.FirstOrDefault(x => x.Id == metadataDeviceState.ID);
		//                                        if (driverState != null)
		//                                        {
		//                                            var deviceDriverState = journalItem.Device.DeviceState.States.FirstOrDefault(x => x.DriverState.Id == driverState.Id);
		//                                            if (deviceDriverState != null)
		//                                            {
		//                                                journalItem.Device.DeviceState.States.Remove(deviceDriverState);
		//                                            }
		//                                        }
		//                                    }
		//                                }
		//                            }
		//                        }
		//                    }
		//                }
		//            }
		//        }
		//        //journalItem.Device.DeviceState.States = new List<DeviceDriverState>();
		//        //Trace.WriteLine(journalItem.Device.DottedPresentationNameAndAddress + " - " + journalItem.StateWord.ToString());
		//    }
		//    // read device 80 byte
		//}
	}
}