using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FS2Api;
using Rubezh2010;
using ServerFS2.Helpers;
using ServerFS2.Service;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		public bool CanNotifyClients = true;

		public void UpdatePanelState(Device panel)
		{
			var states = new List<DeviceDriverState>();
			var statusBytes = ServerHelper.GetPanelStatus(panel);
			if (statusBytes != null && statusBytes.Count == 8)
			{
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
					ForseUpdateDeviceStates(panel);
				}
				UpdateRealChildrenStateOnPanelState(panel, bitArray);
			}
		}

		void UpdateRealChildrenStateOnPanelState(Device panelDevice, BitArray bitArray)
		{
			foreach (var device in panelDevice.GetRealChildren())
			{
				foreach (var metadataDeviceState in MetadataHelper.GetMetadataDeviceStates(device))
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
										ForseUpdateDeviceStates(device);
									}
								}
							}
						}
					}
				}
			}
		}

		public DeviceConfiguration RemoteDeviceConfiguration { get; private set; }

		public bool ReadConfigurationAndUpdateStates(Device panelDevice)
		{
			var getConfigurationOperationHelper = new GetConfigurationOperationHelper(true);
			RemoteDeviceConfiguration = getConfigurationOperationHelper.GetDeviceConfiguration(panelDevice);
			if (RemoteDeviceConfiguration == null)
				return false;
			RemoteDeviceConfiguration.Update();
			var remoteRealChildren = RemoteDeviceConfiguration.RootDevice.GetRealChildren();
			var localRealChildren = panelDevice.GetRealChildren();

			var areEqual = ConfigurationCompareHelper.Compare(panelDevice, RemoteDeviceConfiguration);
			if (!areEqual)
			{
				panelDevice.DeviceState.IsDBMissmatch = true;
				ForseUpdateDeviceStates(panelDevice);
			}

			foreach (var remoteDevice in remoteRealChildren)
			{
				var device = localRealChildren.FirstOrDefault(x => x.IntAddress == remoteDevice.IntAddress);
				if (device != null)
				{
					device.StateWordOffset = remoteDevice.StateWordOffset;
					device.StateWordBytes = remoteDevice.StateWordBytes;
					device.RawParametersOffset = remoteDevice.RawParametersOffset;
					device.RawParametersBytes = remoteDevice.RawParametersBytes;
					ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
				}
			}

			foreach (var remoteZone in RemoteDeviceConfiguration.Zones)
			{
				var zone = ConfigurationManager.Zones.FirstOrDefault(x => x.No == remoteZone.No);
				if (zone != null)
				{
					zone.LocalDeviceNo = remoteZone.LocalDeviceNo;
				}
			}

			return true;
		}

		void ParseDeviceState(Device device, List<byte> stateWordBytes, List<byte> rawParametersBytes)
		{
			ParseStateWordBytes(device, stateWordBytes);
			ParseRawParametersBytes(device, rawParametersBytes);
		}

        void ParseDeviceState(Device device)
        {
            ParseDeviceState(device, device.StateWordBytes, device.RawParametersBytes);
        }

		void ParseStateWordBytes(Device device, List<byte> stateWordBytes)
		{
            if (stateWordBytes == null || stateWordBytes.Count <= 0)
				return;
			BitArray stateWordBitArray = new BitArray(stateWordBytes.ToArray());
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
			{
				if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == tableNo)
				{
					var bitNo = MetadataHelper.GetBitNo(metadataDeviceState);
					if (bitNo != -1 && bitNo < stateWordBitArray.Count)
					{
						var hasBit = stateWordBitArray[bitNo];
						SetStateFromMetadata(device, metadataDeviceState, hasBit);
					}
				}
			}
		}

		void ParseRawParametersBytes(Device device, List<byte> rawParametersBytes)
		{
			if (rawParametersBytes == null || rawParametersBytes.Count <= 1)
				return;
			BitArray rawParametersBitArray = new BitArray(new byte[] { rawParametersBytes[1], rawParametersBytes[0] });

			var metadataDeviceStates = MetadataHelper.GetMetadataDeviceStates(device);

			if (device.Driver.DriverType == DriverType.AM1_O && device.AddressOnShleif == 2)
			{
				;
			}

			foreach (var metadataDeviceState in metadataDeviceStates)
			{
				var intBitNo = MetadataHelper.GetIntBitNo(metadataDeviceState);
				if (intBitNo != -1 && intBitNo < rawParametersBitArray.Count)
				{
					var hasBit = rawParametersBitArray[intBitNo];
					SetStateFromMetadata(device, metadataDeviceState, hasBit);
				}
			}
		}

		void SetStateFromMetadata(Device device, driverConfigDeviceStatesDeviceState metadataDeviceState, bool hasBit)
		{
			//if (device.Driver.DriverType == DriverType.AM1_O && metadataDeviceState.type != "Security" && metadataDeviceState.type != "security")
			//    return;

			if (metadataDeviceState.inverse == "1")
				hasBit = !hasBit;

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
						ForseUpdateDeviceStates(device);
					}
				}
			}
			else
			{
				if (device.DeviceState.States.RemoveAll(x => x.DriverState.Code == metadataDeviceState.ID) > 0)
					ForseUpdateDeviceStates(device);
			}
		}

		public void UpdateDeviceStateOnJournal(List<FS2JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (journalItem.Device != null)
				{
					var metadataDeviceTable = MetadataHelper.GetMetadataDeviceTable(journalItem.Device);
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
										var eventValue = MetadataHelper.GetDeviceStateEventEnter(deviceStateEnter, journalItem.AdditionalEventCode);
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
														ForseUpdateDeviceStates(journalItem.Device);
													}
												}
											}
										}
									}
								}
							}
						}
					}
					ParceDeviceOrZoneLeave(journalItem, journalItem.Device);
					UpdateDeviceStateAndParameters(journalItem.Device);
				}
				if (journalItem.Zone != null)
				{
					foreach (var device in journalItem.Zone.DevicesInZone)
					{
						ParceDeviceOrZoneLeave(journalItem, device);
					}
				}
			}
		}

		void ParceDeviceOrZoneLeave(FS2JournalItem journalItem, Device device)
		{
			var metadataDeviceTable = MetadataHelper.GetMetadataDeviceTable(device);
			if (metadataDeviceTable != null)
			{
				foreach (var metadataDeviceState in MetadataHelper.Metadata.deviceStates)
				{
					if (metadataDeviceState.tableType == null || metadataDeviceState.tableType == metadataDeviceTable.tableType)
					{
						if (metadataDeviceState.leave != null)
						{
							foreach (var deviceStateLeave in metadataDeviceState.leave)
							{
								var eventValue = MetadataHelper.GetDeviceStateEventLeave(deviceStateLeave, journalItem.AdditionalEventCode);
								if (eventValue == null)
								{
									eventValue = MetadataHelper.GetZoneStateEventLeave(deviceStateLeave, journalItem.AdditionalEventCode);
								}
								if (eventValue != null)
								{
									if (eventValue == "$" + journalItem.EventCode.ToString("X2"))
									{
										var driverState = device.Driver.States.FirstOrDefault(x => x.Code == metadataDeviceState.ID);
										if (driverState != null)
										{
											var deviceDriverState = device.DeviceState.States.FirstOrDefault(x => x.DriverState.Code == driverState.Code);
											if (deviceDriverState != null)
											{
												device.DeviceState.States.Remove(deviceDriverState);
												ForseUpdateDeviceStates(device);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			UpdateDeviceStateAndParameters(device);
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

		public void ForseUpdateDeviceStates(Device device)
		{
			SetSerializableStates(device);
			ChangedDeviceStates = new HashSet<DeviceState>();
			PropogateStatesDown();
			PropogateStatesUp();
			device.DeviceState.SerializableStates = device.DeviceState.States;

			ZoneStateManager.ChangeOnDeviceState(!CanNotifyClients);
			if (CanNotifyClients)
			{
				NotifyStateChanged(device);
				CallbackManager.DeviceStateChanged(ChangedDeviceStates.ToList());
				foreach (var deviceState in ChangedDeviceStates)
				{
					deviceState.OnStateChanged();
				}
			}
		}

		public void NotifyStateChanged(Device device)
		{
			if (device.Driver.DriverType == DriverType.AM1_O && device.AddressOnShleif == 1)
			{
				var count = device.DeviceState.States.Count;
				Trace.WriteLine("device.DeviceState.States.Count = " + count);
				;
			}
			CallbackManager.DeviceStateChanged(new List<DeviceState>() { device.DeviceState });
			device.DeviceState.OnStateChanged();
		}

		void SetSerializableStates(Device device)
		{
			if (device.IsMonitoringDisabled)
			{
				AddDeviceState(device, "Мониторинг устройства отключен");
			}
			else
			{
				RemoveDeviceState(device, "Мониторинг устройства отключен");
			}

			if (device.DeviceState.IsUsbConnectionLost)
			{
				AddDeviceState(device, "USB устройство отсутствует");
			}
			else
			{
				RemoveDeviceState(device, "USB устройство отсутствует");
			}

			if (device.DeviceState.IsPanelConnectionLost)
			{
				AddDeviceState(device, "Потеря связи с прибором");
			}
			else
			{
				RemoveDeviceState(device, "Потеря связи с прибором");
			}

			if (device.DeviceState.IsInitializing)
			{
				AddDeviceState(device, "Устройство инициализируется");
			}
			else
			{
				RemoveDeviceState(device, "Устройство инициализируется");
			}

			if (device.DeviceState.IsDBMissmatch || device.DeviceState.IsWrongPanel)
			{
				AddDeviceState(device, "База данных прибора не соответствует базе данных ПК");
			}
			else
			{
				RemoveDeviceState(device, "База данных прибора не соответствует базе данных ПК");
			}
		}

		void AddDeviceState(Device device, string stateName)
		{
			var state = device.Driver.States.FirstOrDefault(x => x.Name == stateName);
			if (state != null)
			{
				var deviceDriverState = new DeviceDriverState
				{
					DriverState = state,
					Time = DateTime.Now
				};
				if (!device.DeviceState.States.Any(x => x.DriverState.Name == stateName))
					device.DeviceState.States.Add(deviceDriverState);
			}
		}

		void RemoveDeviceState(Device device, string stateName)
		{
			device.DeviceState.States.RemoveAll(x => x.DriverState.Name == stateName);
		}

		HashSet<DeviceState> ChangedDeviceStates;

		void PropogateStatesDown()
		{
			foreach (var device in ConfigurationManager.Devices)
			{
				device.DeviceState.ParentStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var device in ConfigurationManager.Devices)
			{
				foreach (var state in device.DeviceState.States.Where(x => x.DriverState.AffectChildren))
				{
					var allChildren = device.GetAllChildren();
					foreach (var childDevice in allChildren)
					{
						var parentDeviceState = new ParentDeviceState()
						{
							ParentDeviceUID = device.UID,
							ParentDevice = device,
							DriverState = state.DriverState,
							IsDeleting = false
						};

						var existingParentDeviceState = childDevice.DeviceState.ParentStates.FirstOrDefault(x => x.ParentDevice.UID == parentDeviceState.ParentDevice.UID && x.DriverState.Code == parentDeviceState.DriverState.Code && x.DriverState == parentDeviceState.DriverState);
						if (existingParentDeviceState == null)
						{
							childDevice.DeviceState.ParentStates.Add(parentDeviceState);
							ChangedDeviceStates.Add(childDevice.DeviceState);
						}
						else
						{
							existingParentDeviceState.IsDeleting = false;
						}

					}
				}
			}

			foreach (var device in ConfigurationManager.Devices)
			{
				var removedCount = device.DeviceState.ParentStates.RemoveAll(x => x.IsDeleting);
				if (removedCount > 0)
				{
					ChangedDeviceStates.Add(device.DeviceState);
				}
			}
		}

		void PropogateStatesUp()
		{
			foreach (var device in ConfigurationManager.Devices)
			{
				device.DeviceState.ChildStates.ForEach(x => x.IsDeleting = true);
			}

			foreach (var device in ConfigurationManager.Devices)
			{
				if (device.Driver.ChildAddressReserveRangeCount == 0)
					continue;

				ChildDeviceState childDeviceState = null;
				var minChildStateType = StateType.Norm;
				foreach (var child in device.Children)
				{
					if (child.DeviceState.StateType < minChildStateType)
					{
						minChildStateType = child.DeviceState.StateType;
						childDeviceState = new ChildDeviceState()
						{
							ChildDeviceUID = device.UID,
							ChildDevice = device,
							StateType = minChildStateType,
							IsDeleting = false
						};
					}
				}

				if (childDeviceState != null)
				{
					var existingDeviceState = device.DeviceState.ChildStates.FirstOrDefault(x => x.ChildDevice.UID == childDeviceState.ChildDevice.UID && x.StateType == childDeviceState.StateType);
					if (existingDeviceState == null)
					{
						device.DeviceState.ChildStates.Add(childDeviceState);
						ChangedDeviceStates.Add(device.DeviceState);
					}
					else
					{
						existingDeviceState.IsDeleting = false;
					}
				}
			}

			foreach (var device in ConfigurationManager.Devices)
			{
				var removedCount = device.DeviceState.ChildStates.RemoveAll(x => x.IsDeleting);
				if (removedCount > 0)
				{
					ChangedDeviceStates.Add(device.DeviceState);
				}
			}
		}
	}
}