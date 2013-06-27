using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using Rubezh2010;
using ServerFS2.Service;
using ServerFS2.Helpers;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		public bool CanNotifyClients = false;

		public void UpdatePanelState(Device panel, bool isSilent = false)
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
			if (SetNewDeviceStates(panel, states))
			{
				ChangeDeviceStates(panel, isSilent);
			}
			UpdateRealChildrenStateOnPanelState(panel, bitArray);
		}

		void UpdateRealChildrenStateOnPanelState(Device panelDevice, BitArray bitArray, bool isSilent = false)
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
										ChangeDeviceStates(device, isSilent);
									}
								}
							}
						}
					}
				}
			}
		}

		public void UpdatePanelChildrenStates(Device panelDevice, bool isSilent = false)
		{
			var getConfigurationOperationHelper = new GetConfigurationOperationHelper(true);
			var remoteDeviceConfiguration = getConfigurationOperationHelper.GetDeviceConfiguration(panelDevice);
			remoteDeviceConfiguration.Update();
			var realChildren = remoteDeviceConfiguration.RootDevice.GetRealChildren();
			panelDevice.DeviceState.IsDBMissmatch = !ConfigurationCompareHelper.Compare(panelDevice, realChildren);
			foreach (var remoteDevice in realChildren)
			{
				var device = ConfigurationManager.Devices.FirstOrDefault(x => x.ParentPanel != null && x.ParentPanel == panelDevice && x.IntAddress == remoteDevice.IntAddress);
				if (device != null)
				{
					device.StateWordOffset = remoteDevice.StateWordOffset;
					device.StateWordBytes = remoteDevice.StateWordBytes;
					device.RawParametersOffset = remoteDevice.RawParametersOffset;
					device.RawParametersBytes = remoteDevice.RawParametersBytes;
					ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes, isSilent);
				}
			}
		}

		void ParseDeviceState(Device device, List<byte> stateWordBytes, List<byte> rawParametersBytes, bool isSilent = false)
		{
			ParseStateWordBytes(device, stateWordBytes, isSilent);

			ParseRawParametersBytes(device, rawParametersBytes, isSilent);
		}

		void ParseStateWordBytes(Device device, List<byte> stateWordBytes, bool isSilent)
		{
			if (stateWordBytes == null)
				return;
			BitArray stateWordBitArray = null;
			if (stateWordBytes.Count > 0)
				stateWordBitArray = new BitArray(stateWordBytes.ToArray());
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
					if (stateWordBitArray != null && bitNo != -1 && bitNo < stateWordBitArray.Count)
					{
						var hasBit = stateWordBitArray[bitNo];
						SetStateFromMetadata(device, metadataDeviceState, hasBit, isSilent);
					}
				}
			}
		}

		void ParseRawParametersBytes(Device device, List<byte> rawParametersBytes, bool isSilent)
		{
			if (rawParametersBytes == null)
				return;
			BitArray rawParametersBitArray = null;
			if (rawParametersBytes.Count > 0)
				rawParametersBitArray = new BitArray(new byte[] { rawParametersBytes[1], rawParametersBytes[0] });
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var intBitNo = MetadataHelper.GetIntBitNo(metadataDeviceState);
					if (rawParametersBitArray != null && intBitNo != -1 && intBitNo < rawParametersBitArray.Count)
					{
						var hasBit = rawParametersBitArray[intBitNo];
						SetStateFromMetadata(device, metadataDeviceState, hasBit, isSilent);
					}
				}
			}
		}

		void SetStateFromMetadata(Device device, driverConfigDeviceStatesDeviceState metadataDeviceState, bool hasBit, bool isSilent)
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
						ChangeDeviceStates(device, isSilent);
					}
				}
			}
			else
			{
				if (device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID) > 0)
					ChangeDeviceStates(device, isSilent);
			}
		}

		public void UpdateDeviceStateOnJournal(List<FS2JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.Device != null)
				{
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
														ChangeDeviceStates(journalItem.Device, false);
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
														ChangeDeviceStates(journalItem.Device, false);
													}
												}
											}
										}
									}
								}
							}
						}
					}
					UpdateDeviceStateAndParameters(journalItem.Device);
				}
			}
		}

		public bool SetNewDeviceStates(Device device, List<DeviceDriverState> newStates)
		{
			var hasChanges = false;
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
						hasChanges = true;
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
			var removedCount = device.DeviceState.States.RemoveAll(x => satesToRemove.Any(y => x.DriverState.Id == y.DriverState.Id));
			if (removedCount > 0)
				hasChanges = true;

			return hasChanges;
		}

		public void ChangeDeviceStates(Device device, bool isSilent = false)
		{
			SetSerializableStates(device);
			PropogateStatesDown(device);

			ZoneStateManager.ChangeOnDeviceState(isSilent);
			if (!isSilent)
				NotifyStateChanged(device);
		}

		public void NotifyStateChanged(Device device)
		{
			CallbackManager.DeviceStateChanged(new List<DeviceState>() { device.DeviceState });
			device.DeviceState.OnStateChanged();
		}

		void SetSerializableStates(Device device)
		{
			if (device.IsParentMonitoringDisabled)
			{
				device.DeviceState.SerializableStates = AddDeviceState(device, "Мониторинг устройства отключен");
				return;
			}

			if (device.DeviceState.IsConnectionLost)
			{
				device.DeviceState.SerializableStates = AddDeviceState(device, "Потеря связи с прибором");
				return;
			}

			if (device.DeviceState.IsInitializing)
			{
				device.DeviceState.SerializableStates = AddDeviceState(device, "Устройство инициализируется");
				return;
			}

			device.DeviceState.SerializableStates = device.DeviceState.States;
		}

		List<DeviceDriverState> AddDeviceState(Device device, string stateName)
		{
			var result = new List<DeviceDriverState>();
			var state = device.Driver.States.FirstOrDefault(y => y.Name == stateName);
			if (state != null)
			{
				var deviceDriverState = new DeviceDriverState
				{
					DriverState = state,
					Time = DateTime.Now
				};
				result.Add(deviceDriverState);
			}
			return result;
		}

		void PropogateStatesDown(Device device)
		{

		}

	}
}