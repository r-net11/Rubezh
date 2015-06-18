﻿using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsDBMissmatchDuringMonitoring = false;
		JournalEventDescriptionType DBMissmatchDuringMonitoringReason;

		bool _isJournalAnyDBMissmatch = false;
		bool IsJournalAnyDBMissmatch
		{
			get { return _isJournalAnyDBMissmatch; }
			set
			{
				if (_isJournalAnyDBMissmatch != value)
				{
					_isJournalAnyDBMissmatch = value;
					var journalItem = new JournalItem()
					{
						JournalEventNameType = value ? JournalEventNameType.База_данных_прибора_не_соответствует_базе_данных_ПК : JournalEventNameType.База_данных_прибора_соответствует_базе_данных_ПК,
					};
					//var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
					//if (!string.IsNullOrEmpty(gkIpAddress))
					//    journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkIpAddress.ToString()));
					AddJournalItem(journalItem);
				}
			}
		}

		void GetAllStates()
		{
			IsDBMissmatchDuringMonitoring = false;
			GKProgressCallback progressCallback = GKProcessorManager.StartProgress("Опрос объектов ГК", "", GkDatabase.Descriptors.Count, false, GKProgressClientType.Monitor);

			foreach (var descriptor in GkDatabase.Descriptors)
			{
				LastUpdateTime = DateTime.Now;
				GetState(descriptor.GKBase);
				if (!IsConnected)
				{
					break;
				}
				GKProcessorManager.DoProgress(descriptor.GKBase.DescriptorPresentationName, progressCallback);

				WaitIfSuspending();
				if (IsStopping)
					return;
			}
			GKProcessorManager.StopProgress(progressCallback);

			CheckTechnologicalRegime();
			NotifyAllObjectsStateChanged();
			IsJournalAnyDBMissmatch = IsDBMissmatchDuringMonitoring;
		}

		void CheckDelays()
		{
			foreach (var device in GKManager.Devices)
			{
				if (!device.Driver.IsGroupDevice && device.AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU))
				{
					CheckDelay(device);
				}
			}
			foreach (var pumpStation in GKManager.PumpStations)
			{
				CheckDelay(pumpStation);
			}
			foreach (var mpt in GKManager.MPTs)
			{
				CheckDelay(mpt);
			}
		}

		void CheckDelay(GKBase gkBase)
		{
			if (gkBase.InternalState == null)
				return;

			bool mustGetState = false;
			switch (gkBase.InternalState.StateClass)
			{
				case XStateClass.TurningOn:
					mustGetState = (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.On:
					mustGetState = gkBase.InternalState.ZeroHoldDelayCount < 10 && (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
					break;
				case XStateClass.TurningOff:
					mustGetState = (DateTime.Now - gkBase.InternalState.LastDateTime).TotalMilliseconds > 500;
					break;
			}
			if (mustGetState)
			{
				var onDelay = gkBase.InternalState.OnDelay;
				var holdDelay = gkBase.InternalState.HoldDelay;
				var offDelay = gkBase.InternalState.OffDelay;

				if (MeasureDeviceInfos.Any(x => x.Device.UID == gkBase.UID))
				{
					GetDelays(gkBase);
				}
				else
				{
					GetState(gkBase, true);
				}

				if (onDelay != gkBase.InternalState.OnDelay || holdDelay != gkBase.InternalState.HoldDelay || offDelay != gkBase.InternalState.OffDelay)
					OnObjectStateChanged(gkBase);

				if (gkBase.InternalState.StateClass == XStateClass.On && holdDelay == 0)
					gkBase.InternalState.ZeroHoldDelayCount++;
				else
					gkBase.InternalState.ZeroHoldDelayCount = 0;
			}
		}

		bool GetDelays(GKBase gkBase)
		{
			SendResult sendResult = null;
			var expectedBytesCount = 68;
			if (gkBase.KauDatabaseParent != null)
			{
				sendResult = SendManager.Send(gkBase.KauDatabaseParent, 2, 12, 32, BytesHelper.ShortToBytes(gkBase.KAUDescriptorNo));
				expectedBytesCount = 32;
			}
			else
			{
				sendResult = SendManager.Send(gkBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(gkBase.GKDescriptorNo));
				expectedBytesCount = 68;
			}

			if (sendResult.HasError || sendResult.Bytes.Count != expectedBytesCount)
			{
				ConnectionChanged(false);
				return false;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, gkBase);

			gkBase.InternalState.LastDateTime = DateTime.Now;
			gkBase.InternalState.OnDelay = descriptorStateHelper.OnDelay;
			gkBase.InternalState.HoldDelay = descriptorStateHelper.HoldDelay;
			gkBase.InternalState.OffDelay = descriptorStateHelper.OffDelay;
			return true;
		}

		void GetState(GKBase gkBase, bool delaysOnly = false)
		{
			var sendResult = SendManager.Send(gkBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(gkBase.GKDescriptorNo));
			if (sendResult.HasError || sendResult.Bytes.Count != 68)
			{
				ConnectionChanged(false);
				return;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, gkBase);
			CheckDBMissmatch(gkBase, descriptorStateHelper);

			gkBase.InternalState.LastDateTime = DateTime.Now;
			if (!delaysOnly)
			{
				gkBase.InternalState.StateBits = descriptorStateHelper.StateBits;
				gkBase.InternalState.AdditionalStates = descriptorStateHelper.AdditionalStates;
			}
			gkBase.InternalState.OnDelay = descriptorStateHelper.OnDelay;
			gkBase.InternalState.HoldDelay = descriptorStateHelper.HoldDelay;
			gkBase.InternalState.OffDelay = descriptorStateHelper.OffDelay;
		}

		void CheckDBMissmatch(GKBase gkBase, DescriptorStateHelper descriptorStateHelper)
		{
			bool isMissmatch = false;
			if (gkBase is GKDevice)
			{
				var device = gkBase as GKDevice;
				if (device.Driver.DriverTypeNo != descriptorStateHelper.TypeNo)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_устройства;
				}

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != GKDriverType.GK && device.DriverType != GKDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != descriptorStateHelper.PhysicalAddress)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_физический_адрес_устройства;
				}

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_адрес_на_контроллере;
				}
			}
			if (gkBase is GKPumpStation)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_НС;
				}
			}
			if (gkBase is GKMPT)
			{
				if (descriptorStateHelper.TypeNo != 0x106)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_МПТ;
				}
			}

			if (gkBase is GKPim)
			{
				if (descriptorStateHelper.TypeNo != 0x107)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_ПИМ;
				}
			}
			if (gkBase is GKDoor)
			{
				if (descriptorStateHelper.TypeNo != 0x104)
				{
					isMissmatch = true;
					DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_тип_для_кода;
				}
			}

			var stringLength = Math.Min(gkBase.PresentationName.Length, 32);
			var description = gkBase.PresentationName.Substring(0, stringLength);
			if (description.TrimEnd(' ') != descriptorStateHelper.Description)
			{
				isMissmatch = true;
				DBMissmatchDuringMonitoringReason = JournalEventDescriptionType.Не_совпадает_описание_компонента;
			}

			gkBase.InternalState.IsDBMissmatchDuringMonitoring = isMissmatch;
			if (isMissmatch)
			{
				IsDBMissmatchDuringMonitoring = true;
			}
		}

		#region TechnologicalRegime
		bool CheckTechnologicalRegime()
		{
			var isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(GkDatabase.RootDevice);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				descriptor.GKBase.InternalState.IsInTechnologicalRegime = isInTechnologicalRegime;
			}

			if (!isInTechnologicalRegime)
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					var isKAUInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(kauDatabase.RootDevice);
					foreach (var device in kauDatabase.RootDevice.AllChildrenAndSelf)
					{
						device.InternalState.IsInTechnologicalRegime = isKAUInTechnologicalRegime;
					}
				}
			}
			return isInTechnologicalRegime;
		}
		#endregion

		void NotifyAllObjectsStateChanged()
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == GkDatabase.RootDevice.UID);
			if (gkControllerDevice != null)
			{
				foreach (var device in gkControllerDevice.AllChildrenAndSelf)
				{
					OnObjectStateChanged(device);
				}
				foreach (var device in gkControllerDevice.AllChildrenAndSelf)
				{
					if (device.Driver.IsGroupDevice || device.DriverType == GKDriverType.RSR2_KAU_Shleif)
					{
						OnObjectStateChanged(device);
					}
				}
				foreach (var pumpStation in GKManager.PumpStations)
				{
					if (pumpStation.GkDatabaseParent == gkControllerDevice)
					{
						OnObjectStateChanged(pumpStation);
					}
				}
				foreach (var mpt in GKManager.MPTs)
				{
					if (mpt.GkDatabaseParent == gkControllerDevice)
					{
						OnObjectStateChanged(mpt);
					}
				}
				foreach (var pim in GKManager.AutoGeneratedPims)
				{
					if (pim.GkDatabaseParent == gkControllerDevice)
					{
						OnObjectStateChanged(pim);
					}
				}
				foreach (var door in GKManager.Doors)
				{
					if (door.GkDatabaseParent == gkControllerDevice)
					{
						OnObjectStateChanged(door);
					}
				}
			}
		}
	}
}